using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;



public class DropDown : MonoBehaviour
{
    public CreateGrid gridImporter;
    public float3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gridcreator = GameObject.Find("GridCreator");
        gridImporter = gridcreator.GetComponent<CreateGrid>();


        vertices = gridImporter.vertices;

        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();

        List<string> items = new List<string>(); 
        items.Add("Choose parameter");
        items.Add("slope");
        items.Add("aspect");
        items.Add("relative slope");
        items.Add("relative aspect");
        items.Add("relative height");
        items.Add("curve");
        items.Add("run-off average");
        items.Add("run-off all cells");
        items.Add("run-off iterate");
        items.Add("distance to skeleton");
        items.Add("angle to skeleton");
        items.Add("profile curvature");
        items.Add("planform curvature");
        items.Add("skeleton length");





        foreach (var item in items)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = item });

        }
        DropdownItemSelected(dropdown);

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    
    void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        switch (index)
        {
            case 0:
                break;
            case 1:
                gridImporter.setMeshSlopeColors();
                break;
            case 2:
                gridImporter.setMeshAspectColors();
                break;
            case 3:
                gridImporter.setMeshAverageSlopeColors();
                break;
            case 4:
                gridImporter.setMeshRelativeAspectColors();
                break;
           case 5:
                gridImporter.setMeshRelativeHeightColors();
                break;
            case 6:
                gridImporter.setMeshCurveColors();
                break;
            case 7:
                gridImporter.setMeshAverageRunoffColors(gridImporter.grid);
                break;
            case 8:
                int ind = 0;
                int[] array = new int[vertices.Length];
                while (ind < vertices.Length)
                {
                    array[ind] = ind;
                    ind++;
                }
                gridImporter.setMeshRunoffColors(array, 3000, 20f);
                break;
            case 9:
                gridImporter.StartCoroutine(gridImporter.iterate(1000));
                break;
            case 10:
                gridImporter.setMeshdLN1Colors();
                break;
            case 11:
                gridImporter.setMeshSkeletonAspectColors();
                break;
            case 12:
                gridImporter.setMeshProfileCurveColors();
                break;
            case 13:
                gridImporter.setMeshPlanformCurveColors();
                break;
            case 14:
                gridImporter.setMeshSkeletonLengthColors();
                break;
            default:
                // code block
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
