using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeastSquares;
using Microsoft.Unity.VisualStudio.Editor;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeastSquares
{
    /// <summary>
    /// Script to fill the leaderboard UI from the Steam leaderboard
    /// </summary>
    public class LeaderboardUI : MonoBehaviour
    {
        public int EntriesToShowAtOnce = 100;
        public GameObject EntryPrefab;
        public TMP_InputField Input;
        public SteamLeaderboard Leaderboard;
        public LeaderboardType Type = LeaderboardType.Global;
        private List<GameObject> _rows = new();
        private int _offset;

        void Start()
        {
            RefreshScores();
        }

        /// <summary>
        /// Fill the leaderboardUI with new scores
        /// </summary>
        public async void RefreshScores()
        {
            LeaderboardEntry[] scores;
            switch (Type)
            {
                case LeaderboardType.Global:
                    scores = await Leaderboard.GetScores(EntriesToShowAtOnce - 1, 1 + _offset);
                    break;
                case LeaderboardType.Friends:
                    scores = await Leaderboard.GetScoresFromFriends();
                    break;
                case LeaderboardType.AroundUser:
                    scores = await Leaderboard.GetScoresAroundUser(EntriesToShowAtOnce / 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RegenerateUI(scores);
        }

        async Task<Sprite> GetLocalUserAvatarSprite()
        {
            var mySteamId = SteamClient.SteamId;  // ① 현재 로그인된 계정의 SteamID
            //
            //    true를 두 번째 인자로 주면, 캐시에 없을 때 서버에서 강제로 받아옵니다.
            bool infoRequested = SteamFriends.RequestUserInformation(mySteamId, true);

            // ③ 아바타 이미지 로드
            var avatarImage = await SteamFriends.GetSmallAvatarAsync(mySteamId);
            if (!avatarImage.HasValue)
                return null;  // 아바타가 없거나 로딩 실패

            // ④ Data.Image → Texture2D 변환
            var tex = avatarImage.Value.Convert();

            // ⑤ Texture2D → Sprite 생성
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        /// <summary>
        /// Renegenerate the leaderboard rows
        /// </summary>
        /// <param name="scores">An array of leaderboard entries</param>
        async void RegenerateUI(LeaderboardEntry[] scores)
        {
            var oldRows = _rows;
            _rows = new List<GameObject>();
            for (var i = 0; i < scores.Length; i++)
            {
                var go = await CreateRow(scores[i]);

                _rows.Add(go);
            }

            // 3) 예외적으로 '내 순위' 한 줄 추가
            LeaderboardEntry[] around = await Leaderboard.GetScoresAroundUser(500); // ±100등 가져오기
            var mySteamId = SteamClient.SteamId;

            // ① 먼저 내 엔트리를 찾는다 (ID 기준)
            LeaderboardEntry myEntry = default;
            bool found = false;

            foreach (var entry in around)
            {
                if (entry.User.Id == mySteamId)
                {
                    myEntry = entry;
                    found = true;
                    break;
                }
            }

            GameObject userRow;

            if (found)
            {
                // ② 내 정확한 엔트리를 찾았을 때
                userRow = await CreateRow(myEntry);
            }
            else
            {
                // ③ 기록이 없거나 탐색 실패했을 때
                userRow = Instantiate(EntryPrefab, transform);
                var row = userRow.GetComponent<LeaderboardUIRow>();
                row.Rank.text = "None";
                row.Score.text = "None";
                row.Name.text = SteamClient.Name;

                var sprite = await GetLocalUserAvatarSprite();
                row.Avatar.sprite = sprite;
            }

            userRow.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
            //
            _rows.Add(userRow);


            for (var i = 0; i < oldRows.Count; i++)
            {
                Destroy(oldRows[i]);
            }
        }

        /// <summary>
        /// Create a row for the leaderboard entry
        /// </summary>
        /// <param name="entry">The given LeaderboardEntry</param>
        /// <returns>A GameObject representing the row</returns>
        private async Task<GameObject> CreateRow(LeaderboardEntry entry)
        {
            var go = Instantiate(EntryPrefab, transform);
            var row = go.GetComponent<LeaderboardUIRow>();
            row.Score.text = entry.Score.ToString();
            row.Name.text = entry.User.Name;
            row.Rank.text = "#" + entry.GlobalRank.ToString();
            var maybeImage = await entry.User.GetSmallAvatarAsync();
            if (maybeImage.HasValue)
            {
                var tex2D = maybeImage.Value.Convert();
                row.Avatar.sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), Vector2.zero);
            }

            return go;
        }

        /// <summary>
        /// Upload the score in the text field to the leaderboard. Called from the "Save Score" button
        /// </summary>
        public void SaveScore()
        {
            var text = Input.text;
            Leaderboard.SubmitScore(int.Parse(text));
            RefreshScores();
        }

    }

    [Serializable]
    public enum LeaderboardType
    {
        Global,
        Friends,
        AroundUser
    }
}