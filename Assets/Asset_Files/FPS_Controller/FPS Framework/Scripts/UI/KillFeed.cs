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

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            Tag.gameObject.SetActive(false);
            skull.gameObject.SetActive(false);
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
            KillTag newTag = Instantiate(Tag, tagsHolder);
            //newTag.message.color = headshot && newTag.updateImageColors ? headshotColor : newTag.message.color;
            newTag.gameObject.SetActive(true);

            if(critical == false)
                newTag.Show(null, null, damage, Color.white);
            else
                newTag.Show(null, null, damage, Color.red);

        }
    }
}