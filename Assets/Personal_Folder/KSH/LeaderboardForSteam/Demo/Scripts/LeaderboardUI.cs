using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeastSquares;
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

        async void Start()
        {
            if (!SteamClient.IsValid)
            {
                Debug.LogError("SteamClient is not initialized. Waiting for initialization...");
                await WaitForSteamInitialization();
            }

            await InitializeScoreIfMissing();

            SaveScore();
            RefreshScores();
        }


        private async Task InitializeScoreIfMissing()
        {
            // 주변 순위(내 순위 포함)를 한 번 가져와서 내 SteamId가 있는지 확인
            var around = await Leaderboard.GetScoresAroundUser(EntriesToShowAtOnce / 2);
            bool hasMyEntry = around != null && around.Any(e => e.User.Id == SteamClient.SteamId);

            if (!hasMyEntry)
            {
                Debug.Log("리더보드에 내 기록이 없습니다. 로컬 스코어를 0으로 초기화합니다.");
                // RecordManager 쪽에서 PlayerPrefs.SetInt + Save까지 처리되도록 구현한 메서드 사용
                
                PlayerPrefs.SetInt("InfiniteMaxStage", 0);
                PlayerPrefs.Save();

                Leaderboard.SubmitScore(0);
            }
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
            if (!SteamClient.IsValid)
            {
                Debug.LogError("SteamClient is not valid. Cannot load user avatar.");
                return null;
            }

            var mySteamId = SteamClient.SteamId;
            bool infoRequested = SteamFriends.RequestUserInformation(mySteamId, true);
            var avatarImage = await SteamFriends.GetSmallAvatarAsync(mySteamId);
            if (!avatarImage.HasValue)
                return null;

            var tex = avatarImage.Value.Convert();
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Renegenerate the leaderboard rows
        /// </summary>
        /// <param name="scores">An array of leaderboard entries</param>
        async void RegenerateUI(LeaderboardEntry[] scores)
        {
            if (!SteamClient.IsValid)
            {
                Debug.LogError("SteamClient is not valid. Cannot regenerate leaderboard UI.");
                return;
            }

            var oldRows = _rows;
            _rows = new List<GameObject>();
            var mySteamId = SteamClient.SteamId;

            // 중복 제거
            var uniqueScores = scores.Distinct(new LeaderboardEntryComparer()).ToArray();
            bool hasMyEntry = uniqueScores.Any(e => e.User.Id == mySteamId);

            // UI 행 생성
            for (var i = 0; i < uniqueScores.Length; i++)
            {
                var entry = uniqueScores[i];
                var go = await CreateRow(entry);
                if (i < 9 && entry.User.Id == mySteamId)
                {
                    var bgImage = go.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
                    bgImage.color = UnityEngine.Color.green;
                }
                _rows.Add(go);
            }

            // "내 순위" 추가 (중복 방지)
            if (!hasMyEntry)
            {
                var around = await Leaderboard.GetScoresAroundUser(EntriesToShowAtOnce / 2);
                if (around != null && around.Length > 0)
                {
                    var myEntry = around.FirstOrDefault(e => e.User.Id == mySteamId);
                    if (myEntry.User.Id == mySteamId && !uniqueScores.Any(e => e.User.Id == mySteamId))
                    {
                        GameObject userRow = await CreateRow(myEntry);
                        userRow.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                        _rows.Add(userRow);
                    }
                }
            }

            foreach (var old in oldRows)
                Destroy(old);
        }

        // LeaderboardEntry 비교를 위한 커스텀 comparer
        private class LeaderboardEntryComparer : IEqualityComparer<LeaderboardEntry>
        {
            public bool Equals(LeaderboardEntry x, LeaderboardEntry y)
            {
                return x.User.Id == y.User.Id && x.Score == y.Score;
            }

            public int GetHashCode(LeaderboardEntry obj)
            {
                return obj.User.Id.GetHashCode() ^ obj.Score.GetHashCode();
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
            
            Leaderboard.SubmitScore(RecordManager.Instance.LoadInfiniteStage());
            RefreshScores();
        }
        private async Task WaitForSteamInitialization()
        {
            int maxAttempts = 10;
            int attempt = 0;
            while (!SteamClient.IsValid && attempt < maxAttempts)
            {
                await Task.Delay(500); // 0.5초 대기
                attempt++;
            }
            if (!SteamClient.IsValid)
            {
                Debug.LogError("Failed to initialize SteamClient after waiting.");
            }
        }
        private void OnEnable()
        {
            _rows.Clear(); // 기존 행 초기화

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