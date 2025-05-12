using UnityEngine;

public class TestCS : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TestFormulas()
    {

        UIManager.Instance?.HandlePuzzleSolved("Puzzle_A");

    }
}
