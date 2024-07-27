using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class BannerUIBehaviour
{

    struct bannerWord
    {
        public string word;
        public float flashTime;
        public bannerWord(string _word, float _flashTime)
        {
            word = _word;
            flashTime = _flashTime;
        }

    }
    TMP_Text bannerText;
    int bannerMessageIdx = -1;
    IEnumerator messageCycle, flashEvent;
    Dictionary<string, bannerWord> bannerMessages = new Dictionary<string, bannerWord>();

    public BannerUIBehaviour(TMP_Text _banner)
    {
        bannerText = _banner;
        ConnectionManager.singleton.RegisterServerEventListener("listen", () => { AddBannerMessage(ConnectionManager.singleton.GetRoomCode()); });
        ConnectionManager.singleton.RegisterServerEventListener("listen", () => { AddBannerMessage("Join Now"); });
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", () => FlashMessage("Lets Get This Party Started", 0.25f));
    }
    public void AddBannerMessage(string _message, float _flashTime = 3)
    {
        bannerMessages[_message] = new bannerWord(_message, _flashTime);
        RecalculateBannerVisibility();
    }

    public void RemoveBannerMessage(string _message)
    {
        bannerMessages.Remove(_message);
        RecalculateBannerVisibility();
    }

    public void ClearBannerMessage()
    {
        bannerMessages.Clear();
        RecalculateBannerVisibility();
    }

    void RecalculateBannerVisibility()
    {
        bannerText.transform.parent.GetComponent<Animator>().SetBool("visibility", bannerMessages.Count > 0);
        bannerMessageIdx = Mathf.Min(bannerMessageIdx, bannerMessages.Count);
        if (messageCycle != null && bannerMessages.Count == 0)
        {
            UIManager.singleton.StopCoroutine(messageCycle);
            messageCycle = null;
            return;
        }
        else if (messageCycle == null && flashEvent == null)
        {
            messageCycle = CycleBannerMessageIdx();
            UIManager.singleton.StartCoroutine(messageCycle);
        }
    }


    IEnumerator CycleBannerMessageIdx()
    {
        DebugLogger.SourcedPrint("Banner", "cycle");
        bannerMessageIdx = (bannerMessageIdx + 1) % bannerMessages.Count;
        bannerText.text = bannerMessages.ElementAt(bannerMessageIdx).Value.word;
        yield return new WaitForSeconds(bannerMessages.ElementAt(bannerMessageIdx).Value.flashTime);
        messageCycle = CycleBannerMessageIdx();
        UIManager.singleton.StartCoroutine(messageCycle);
    }

    Queue<bannerWord> flashQueue = new Queue<bannerWord>();

    public void FlashMessage(string _message, float _flashTime = 5)
    {
        foreach (string word in _message.Split(' ')) flashQueue.Enqueue(new bannerWord(word, _flashTime));
        if (flashEvent != null) return;
        flashEvent = EmptyFlashQueue();
        UIManager.singleton.StartCoroutine(flashEvent);
    }

    IEnumerator EmptyFlashQueue()
    {
        if (messageCycle != null)
        {
            UIManager.singleton.StopCoroutine(messageCycle);
            messageCycle = null;
        }

        while (flashQueue.Count > 0)
        {
            bannerWord currentWord = flashQueue.Dequeue();
            bannerText.text = currentWord.word;
            yield return new WaitForSeconds(currentWord.flashTime);
        }
        flashEvent = null;
        messageCycle = CycleBannerMessageIdx();
        UIManager.singleton.StartCoroutine(messageCycle);

    }
}
