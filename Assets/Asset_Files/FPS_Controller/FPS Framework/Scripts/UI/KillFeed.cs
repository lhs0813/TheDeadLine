using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Kill Feed")]
    public class KillFeed : MonoBehaviour
    {
        public KillTag counter;
        public KillTag Tag;
        public Transform tagsHolder;
        public KillTag skull;
        public RectTransform skullsHolder;
        public bool useSFX;
        public AudioClip killSFX;
        public Color headshotColor = Color.red;

        public AudioSource audioSource;

        private List<KillTag> pooledTags = new List<KillTag>();
        public int maxTagPoolSize = 50;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            Tag.gameObject.SetActive(false);
            skull.gameObject.SetActive(false);

            // 💡 풀링: maxTagPoolSize만큼 미리 생성
            for (int i = 0; i < maxTagPoolSize; i++)
            {
                KillTag tag = Instantiate(Tag, tagsHolder);
                tag.gameObject.SetActive(false);
                pooledTags.Add(tag);
            }
        }

        private KillTag GetAvailableTag()
        {
            foreach (var tag in pooledTags)
            {
                if (!tag.gameObject.activeSelf)
                    return tag;
            }

            // 모두 사용 중이면 가장 오래된 것(타이머 낮은 순) 재사용
            pooledTags.Sort((a, b) => a.TimerValue().CompareTo(b.TimerValue()));
            return pooledTags[0];
        }

        public void Show(Actor killer, string killed, bool headshot)
        {
            counter.Show(killer, killed);

            
            KillTag newSkull = Instantiate(skull, skullsHolder);
            RawImage newSkullImage = newSkull.GetComponent<RawImage>();

            newSkullImage.color = headshot && newSkull.updateImageColors ? headshotColor : newSkullImage.color;
            


            newSkull.gameObject.SetActive(true);
            

            
            newSkull.Show(killer, killed);
            

            audioSource.Stop();

            if (killSFX)
                audioSource.PlayOneShot(killSFX);
        }

        public void DamageShow(float damage, bool critical)
        {
            KillTag newTag = GetAvailableTag();
            newTag.gameObject.SetActive(true);
            newTag.transform.SetAsFirstSibling(); // 🔥 항상 리스트 맨 위에 추가됨

            Color color = critical ? Color.red : Color.white;
            newTag.Show(null, null, damage, color);
        }
    }
}