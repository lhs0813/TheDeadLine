using System;
using DunGen;
using System.Collections.ObjectModel;

    public static class TileExtensions
    {
        /// <summary>
        /// 이 타일이 메인 경로에 있으면 그 인덱스를, 아니면 -1을 반환합니다.
        /// </summary>
        public static int GetDeepness(this Tile tile)
        {
            if (tile == null || tile.Dungeon == null)
                return -1;

            // ReadOnlyCollection<Tile>에도 IndexOf가 있으므로 바로 사용 가능
            var mainPath = tile.Dungeon.MainPathTiles;
            return mainPath.IndexOf(tile);
        }

        /// <summary>
        /// 이 타일이 메인 경로에 포함되어 있는지 여부를 반환합니다.
        /// </summary>
        public static bool IsMainPath(this Tile tile)
        {
            // GetDeepness가 0 이상이면 메인 경로에 있는 것
            return tile.GetDeepness() >= 0;
        }
    }