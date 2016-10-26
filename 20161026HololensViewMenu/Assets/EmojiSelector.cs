using UnityEngine;

public class EmojiSelector : MonoBehaviour
{
    void Start() {}
    void OnGazeEnter() {}
    void OnGazeLeave() {}
    void Update() {}
    void OnSelect()
    {
        var hit = HoloToolkit.Unity.GazeManager.Instance.HitInfo;
        if (hit.transform.gameObject == null) return;
        var gameObjectName = hit.transform.gameObject.name;
        var textResult = GameObject.Find("ResultText");
        textResult.GetComponent<TextMesh>().text = gameObjectName;
    }
}
