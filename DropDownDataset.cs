using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;



public class DropDownDataset : MonoBehaviour
{
    public CreateGrid gridImporter;
    public float3[] vertices;

    // Start is called before the first frame update
    void Start()
    {


        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();

        List<string> items = new List<string>();
        items.Add("Chooeter");
        items.Add("sloe");
        items.Add("aspect");
        items.Add("relative slope");
        items.Add("reive aspect");
        items.Add("relative height");
        items.Add("curve");
        items.Add("run-off random");
        items.Add("run-off all cells");
        items.Add("run-off iterate");


        foreach (var item in items)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = item });

        }
        DropdownItemSelected(dropdown);

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }


    void DropdownItemSelected(Dropdown dropdown)
    {
        GameObject terrain = GameObject.Find("TerrainLoader");
        MeshGenerator meshGenerator = terrain.GetComponent<MeshGenerator>();
        GameObject gridcreator = GameObject.Find("GridCreator");
        gridImporter = gridcreator.GetComponent<CreateGrid>();
        int index = dropdown.value;
        switch (index)
        {
            case 0:
                meshGenerator.StartPipe(meshGenerator.vertexFile2018);
                gridImporter.getData();
                gridImporter.InstantiateGrid();
                gridImporter.WriteString();
                Debug.Log("Output written 2018");
                break;
            case 1:
                meshGenerator.StartPipe(meshGenerator.vertexFile1997);
                gridImporter.getData();
                gridImporter.InstantiateGrid();
                gridImporter.WriteString();
                Debug.Log("Output written 1997");
                break;
            case 2:
                meshGenerator.StartPipe(meshGenerator.vertexFile2008);
                gridImporter.getData();
                gridImporter.InstantiateGrid();
                gridImporter.WriteString();
                Debug.Log("Output written 2008"); break;
            case 3:
                meshGenerator.StartPipe(meshGenerator.vertexFile2012);
                gridImporter.getData();
                gridImporter.InstantiateGrid();
                gridImporter.WriteString();
                Debug.Log("Output written 2012"); break;
            case 4:
                meshGenerator.StartPipe(meshGenerator.vertexFile2018);
                gridImporter.getData();
                gridImporter.InstantiateGrid();
                gridImporter.WriteString();
                Debug.Log("Output written 2018"); break;
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
