using UnityEngine;

public class BoolChange : MonoBehaviour
{

    [SerializeField] private SequenceController plot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    plot.SecondHand = false;
        plot.ThirdHand = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
