using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownController : MonoBehaviour
{

    List<string> algorithms = new List<string> { "A*", "Greedy Best-First", "Depth-First (Jank)" , "Breadth-First"};
    Dropdown algorithmSelection;
    // Start is called before the first frame update

    void Start()
    {
        algorithmSelection = GetComponent<Dropdown>();
        algorithmSelection.ClearOptions();
        algorithmSelection.AddOptions(algorithms);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
