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
        items.Add("Choose Dataset");
        items.Add("1983");
        items.Add("1997");
        items.Add("2008");
        items.Add("2012");
        items.Add("2018");


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
                meshGenerator.StartPipe(meshGenerator.vertexFile2018, 10, false);
                gridImporter.getData();
                gridImporter.InstantiateGrid(gridImporter.mesh, gridImporter.discharge2018C, gridImporter.discharge2018P);
                gridImporter.WriteString();
                Debug.Log("Output written 2018");
                break;
            case 1:
                meshGenerator.StartPipe(meshGenerator.vertexFile1983, 10, false);
                gridImporter.getData();
                gridImporter.InstantiateGrid(gridImporter.mesh, gridImporter.discharge1997C, gridImporter.discharge1997P);
                gridImporter.WriteString();
                Debug.Log("Output written 1983");
                break;
            case 2:
                meshGenerator.StartPipe(meshGenerator.vertexFile1997, 10, false);
                gridImporter.getData();
                gridImporter.InstantiateGrid(gridImporter.mesh, gridImporter.discharge1997C, gridImporter.discharge1997P);
                gridImporter.WriteString();
                Debug.Log("Output written 1997");
                break;
            case 3:
                meshGenerator.StartPipe(meshGenerator.vertexFile2008, 10, false);
                gridImporter.getData();
                gridImporter.InstantiateGrid(gridImporter.mesh, gridImporter.discharge2008C, gridImporter.discharge2008P);
                gridImporter.WriteString();
                Debug.Log("Output written 2008"); break;
            case 4:
                meshGenerator.StartPipe(meshGenerator.vertexFile2012, 10, false);
                gridImporter.getData();
                gridImporter.InstantiateGrid(gridImporter.mesh, gridImporter.discharge2012C, gridImporter.discharge2012P);
                gridImporter.WriteString();
                Debug.Log("Output written 2012"); break;
            case 5:
                meshGenerator.StartPipe(meshGenerator.vertexFile2018, 10, false);
                gridImporter.getData();
                gridImporter.InstantiateGrid(gridImporter.mesh, gridImporter.discharge2018C, gridImporter.discharge2018P);
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
