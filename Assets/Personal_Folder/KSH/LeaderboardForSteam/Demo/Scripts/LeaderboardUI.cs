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
            SaveScore();
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

            var mySteamId = SteamClient.SteamId;

            // 1) scores로부터 UI 행 생성하면서—동시에 내 ID라면 바로 강조
            for (var i = 0; i < scores.Length; i++)
            {
                var entry = scores[i];
                var go = await CreateRow(entry);

                // 1~9번째(인덱스 0~8)만 검사하고 싶다면
                if (i < 9 && entry.User.Id == mySteamId)
                {
                    // 배경 Image 컴포넌트 가져오기
                    var bgImage = go.transform.GetChild(0)
                                     .GetComponent<UnityEngine.UI.Image>();
                    bgImage.color = UnityEngine.Color.green;
                }

                _rows.Add(go);
            }

            // 3) 이미 리스트에 내 엔트리가 있는지 검사
            bool hasMyEntry = scores.Any(e => e.User.Id == mySteamId);
            
            // 4) 없을 때만 “내 순위” 추가
            if (!hasMyEntry)
            {
                var around = await Leaderboard.GetScoresAroundUser(EntriesToShowAtOnce / 2);
                if (around != null && around.Length > 0)
                {
                    // 내 Entry 찾기
                    var myEntry = around.FirstOrDefault(e => e.User.Id == mySteamId);
                    bool found = myEntry.User.Id == mySteamId;

                    GameObject userRow;
                    if (found)
                    {
                        // 내 점수가 주변 순위 안에 있을 때
                        userRow = await CreateRow(myEntry);
                    }
                    else
                    {
                        // 내 점수가 전혀 없을 때
                        userRow = Instantiate(EntryPrefab, transform);
                        var row = userRow.GetComponent<LeaderboardUIRow>();
                        row.Rank.text = "None";
                        row.Score.text = "None";
                        row.Name.text = SteamClient.Name;
                        row.Avatar.sprite = await GetLocalUserAvatarSprite();
                    }

                    // 강조 색상(UnityEngine.Color 명시)
                    userRow.transform.GetChild(0)
                           .GetComponent<UnityEngine.UI.Image>()
                           .color = UnityEngine.Color.green;

                    _rows.Add(userRow);
                }
                else
                {
                    Debug.Log("주변 순위가 없습니다.");
                }
            }
            
                // 5) 이전에 생성했던 행들 정리
                foreach (var old in oldRows)
                    Destroy(old);
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
            
            Leaderboard.SubmitScore(RecordManager.Instance.LoadInfiniteStage());
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