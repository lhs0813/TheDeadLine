// using System.Threading.Tasks;
// using DunGen;
// using DunGen.Graph;
// using UnityEngine;
// using UnityEngine.AddressableAssets;

// public static class MapContextBuilder
// {
//     private static async Task<DungeonFlow> GetDungeonFlowAsync(int mapIndex)
//     {
//         int ModifiedMapIndex = mapIndex > 10 ? 10 : mapIndex;

//         string key = $"DF_Station_{ModifiedMapIndex}";
//         var handle = Addressables.LoadAssetAsync<DungeonFlow>(key);
//         var flow = await handle.Task;

//         if (flow == null)
//         {
//             Debug.LogError($"DungeonFlow '{key}' 가 없습니다.");
//             return null;
//         }
//         else
//         {
//             Debug.Log($"DungeonFlow '{key}' 를 적용했습니다");
//         }

//         // 🔐 안전하게 복사해서 반환
//         return ScriptableObject.Instantiate(flow);
//     }


// }


