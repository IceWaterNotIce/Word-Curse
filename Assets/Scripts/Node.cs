using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    public BoxCollider2D m_collider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_collider.size = new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);
    }

    void OnEnable()
    {

    }

    public void SetWord(Word word)
    {
        GetComponentInChildren<TMPro.TMP_Text>().text = word.word;
    }
}