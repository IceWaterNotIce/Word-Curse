using UnityEngine;
using UnityEngine.SceneManagement;

public class GraphViewSceneManager : MonoBehaviour
{

    [SerializeField] private WordJsonManager dataController;
    public Canvas canvas;
    Word[] words;

    public GameObject nodePrefab;

    async void Start()
    {
        await dataController.LoadFromCloud();
        dataController.SaveToLocal();
        words = dataController.GetWords();

        foreach (Word word in words)
        {
            GameObject node = Instantiate(nodePrefab, canvas.transform);
            node.GetComponent<Node>().SetWord(word);
        }
    }
    // Start is called before the first frame update async void Start()
   
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToLobby()
    {
        // Load the Lobby scene
        SceneManager.LoadScene("Lobby");
    }
}