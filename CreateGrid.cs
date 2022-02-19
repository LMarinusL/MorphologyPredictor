using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;
using System;
using Unity.Mathematics;

public class CreateGrid : MonoBehaviour
{
    public float3[] vertices;
    public float3[] normals;
    public int[] triangles;
    public MATList MATlist;
    public MATBall[] MATcol;
    public Grid grid;
    public Vector2 RM1 = new Vector2(659492f, 1020360f);
    public Vector2 RM2 = new Vector2(654296f, 1023740f);
    public Vector2 RM3 = new Vector2(658537f, 1032590f);
    float xCorrection;
    float zCorrection;
    public int xSize;
    public int zSize;
    public GameObject dotgreen;
    Mesh mesh;
    Color[] colors;




    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            getData();
            InstantiateGrid(vertices, normals, triangles);
            WriteString();
            Debug.Log("Output written");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            setMeshSlopeColors();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            setMeshAspectColors();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            setMeshRelativeSlopeColors();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            setMeshRelativeAspectColors();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            setMeshCurveColors();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            setMeshdLN1Colors();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            setMeshContourColors();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            int index = 0;
            int[] array = new int[vertices.Length];
            while ( index < vertices.Length)
            {
                array[index] = index;
                index++;
            }
            setMeshRunoffColors(array, 3000, 20f);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            List<int> startAt = new List<int>();
            for (int i = 0; i < 1000; i++)
            {
                startAt.Add(UnityEngine.Random.Range(100, 250000));
            }
            setMeshRunoffColors(startAt.ToArray(), 3000, 20f);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(iterate(1000));
        }


    }

    void getData()
    {
        GameObject terrain = GameObject.Find("TerrainLoader");
        MeshGenerator meshGenerator = terrain.GetComponent<MeshGenerator>();
        GameObject MAT = GameObject.Find("MATLoader");
        ShrinkingBallSeg MATalg = MAT.GetComponent<ShrinkingBallSeg>();

        xCorrection = meshGenerator.xCorrection;
        zCorrection = meshGenerator.zCorrection;
        xSize = meshGenerator.xSizer;
        zSize = meshGenerator.zSizer;
        mesh = meshGenerator.mesh;

        vertices = meshGenerator.vertices;
        normals = meshGenerator.normals;
        triangles = meshGenerator.triangles;
        MATlist = MATalg.list;
        MATcol = MATlist.NewMATList;
    }

    public void InstantiateGrid(float3[] verts, float3[] normals, int[] triangles)
    {
        grid = new Grid(verts, normals, triangles);
        foreach (Cell cell in grid.cells)
        {
            //computeCurvatureX(cell);
            //computeCurvatureY(cell);
            computeESRICurvature(cell);
            //Debug.Log(cell.curvature);
            cell.relativeHeight = relativeHeight(cell.index, grid, 1);
            cell.relativeSlope = relativeSlope(cell.index, grid, 1);
            cell.relativeAspect = relativeAspect(cell.index, grid, 1);
            cell.dRM1 = DistTo(cell.x, cell.z, Correct2D(RM1, xCorrection, zCorrection));
            //cell.dLN1 = Mathf.Pow(HandleUtility.DistancePointLine(new float3(cell.x, cell.y, cell.z), vertices[10], vertices[150800]), 2);
        }
    }

    public void WriteString()
    {
        string path = "Assets/Output/outputGrid.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine("x y h slope aspect RM1 RM2 RM3 relativeHeight relativeSlope relativeAspect");
        foreach (Cell cell in grid.cells)
        {
            if(cell.y == 0) { continue; }
            writer.WriteLine(cell.x + " "+ cell.z + " " + cell.y + " " + 
                cell.slope + " " + cell.aspect + " " 
                + DistTo(cell.x, cell.z, Correct2D(RM1, xCorrection, zCorrection))
                + " " + DistTo(cell.x, cell.z, Correct2D(RM2, xCorrection, zCorrection))
                + " " + DistTo(cell.x, cell.z, Correct2D(RM3, xCorrection, zCorrection))
                + " " + HandleUtility.DistancePointLine(new float3(cell.x, cell.y, cell.z), vertices[10], vertices[400])
                + " " + cell.relativeHeight + " " + cell.relativeSlope + " " + cell.relativeAspect
                );
        }
        writer.Close();
    }

    public float DistTo(float x, float y , Vector2 Point)
    {
        float dist = Mathf.Pow((Mathf.Pow(x - Point.x, 2f) + Mathf.Pow(y - Point.y, 2f)), 0.5f);
        return dist;
    }

    public Vector2 Correct2D(Vector2 point, float xcor, float ycor)
    {
        return new Vector2((point.x-xcor)/10 , (point.y - ycor) / 10);
    }

    public List<int> getIndicesOfSurroundingCells(int index, Grid grid, int dist)
    {
        Cell own = grid.cells[index];
        int xLoc = getXFromIndex(index);
        int zLoc = getZFromIndex(index);
        List<int> indices = new List<int>();
        if(xLoc > 0 + dist )
        {
            indices.Add(getIndexFromLoc(xLoc - dist, zLoc));
            if (zLoc > 0 + dist)
            {
                indices.Add(getIndexFromLoc(xLoc - dist, zLoc- dist));
            }
            if (zLoc < (zSize - dist))
            {
                indices.Add(getIndexFromLoc(xLoc - dist, zLoc + dist));
            }
        }
        if (zLoc > 0 + dist)
        {
            indices.Add(getIndexFromLoc(xLoc , zLoc- dist));
        }
        if (zLoc < (zSize - dist))
        {
            indices.Add(getIndexFromLoc(xLoc, zLoc + dist));
        }
        if (xLoc < (xSize- dist))
        {
            indices.Add(getIndexFromLoc(xLoc + dist, zLoc));
            if (zLoc > 0 + dist)
            {
                indices.Add(getIndexFromLoc(xLoc + dist, zLoc - dist));
            }
            if (zLoc < (zSize - dist))
            {
                indices.Add(getIndexFromLoc(xLoc + dist, zLoc + dist));
            }
        }
        return indices;
    }

    float relativeHeight(int index, Grid grid, int dist)
    {
        List<int> indices = getIndicesOfSurroundingCells(index, grid, dist);
        float averageHeight = 0f;
        float heightSum = 0f;
        int numOfCells = 0;
        foreach (int i in indices)
        {
            if (grid.cells[i].y != 0) // only take vertices that are not at the height 0
            {
                heightSum = heightSum + grid.cells[i].y;
                numOfCells++;
            }
        }
        if (numOfCells == 0) // if there are no cells around it that are not at height zero, prevent dividing by zero
        {
            return 0f;
        }
        averageHeight = heightSum / numOfCells;
        float heightOwn = grid.cells[index].y;
        return averageHeight - heightOwn;
    }

    float relativeSlope(int index, Grid grid, int dist)
    {
        List<int> indices = getIndicesOfSurroundingCells(index, grid, dist);
        float averageSlope = 0f;
        float slopeSum = 0f;
        int numOfCells = 0;
        foreach (int i in indices)
        {
            if (grid.cells[i].y != 0) // only take vertices that are not at the height 0
            {
                slopeSum = slopeSum + grid.cells[i].slope;
                numOfCells++;
            }
        }
        if (numOfCells == 0)
        {
            return 0f;
        }
        averageSlope = slopeSum / numOfCells;
        float slopeOwn = grid.cells[index].slope;
        return averageSlope - slopeOwn;
    }

    float relativeAspect(int index, Grid grid, int dist)
    {
        List<int> indices = getIndicesOfSurroundingCells(index, grid, dist);
        float averageAspect = 0f;
        float aspectSum = 0f;
        int numOfCells = 0;
        foreach (int i in indices)
        {
            if (grid.cells[i].y != 0) // only take vertices that are not at the height 0
            {
                aspectSum = aspectSum + grid.cells[i].aspect;
                numOfCells++;
            }
        }
        if (numOfCells == 0)
        {
            return 0f;
        }
        averageAspect = aspectSum / numOfCells;
        float aspectOwn = grid.cells[index].aspect;
        return averageAspect - aspectOwn;
    }

    public int getIndexFromLoc(int xLoc, int zLoc)
    {
        return (xLoc ) + (zLoc * xSize); 
    }
    
    public int getZFromIndex(int index)
    {
        int result = Mathf.FloorToInt(index / xSize);
        return result;
    }

    public int getXFromIndex(int index)
    {
        return index - (getZFromIndex(index) * xSize);
    }

    // COLORS
    // todo: get max values to set colors
    void setMeshSlopeColors()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(1f , 1f * (1 - grid.cells[i].slope/1.52f), 0f, 1f);
        }
        mesh.colors = colors;
    }
    void setMeshAspectColors()
    {
        colors = new Color[vertices.Length]; 
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(1f * (grid.cells[i].aspect/180), 1f * (grid.cells[i].aspect / 180), 1f * ((180 - grid.cells[i].aspect)/180), 1f);
        }
        mesh.colors = colors;
    }
    void setMeshRelativeSlopeColors()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(Mathf.Pow(Mathf.Pow((grid.cells[i].relativeSlope*3), 2f), 0.5f),  Mathf.Pow(Mathf.Pow((grid.cells[i].relativeSlope*3), 2f), 0.5f), Mathf.Pow(Mathf.Pow((grid.cells[i].relativeSlope*3), 2f), 0.5f), 1f);
        }
        mesh.colors = colors;
    }
    void setMeshRelativeAspectColors()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(1f * (grid.cells[i].relativeAspect / 50), 1f * (grid.cells[i].relativeAspect / 50), 1f * (1-((grid.cells[i].relativeAspect) / 50)), 1f);
        }
        mesh.colors = colors;
    }
    void setMeshCurveColors()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(1f * (grid.cells[i].curvature), 1f * (grid.cells[i].curvature ), 1f * (grid.cells[i].curvature), 1f);

        }
        mesh.colors = colors;
    }
    void setMeshdLN1Colors()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(1f * (grid.cells[i].dLN1/10000), 1f * (grid.cells[i].dLN1/10000), 1f * (grid.cells[i].dLN1/10000), 1f);
        }
        mesh.colors = colors;
    }
    void setMeshContourColors()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            if (grid.cells[i].y < 100)
            {
                colors[i] = new Color(1f, 1f , 1f, 1f);
            }
            else {
                colors[i] = new Color(0f, 0f, 0f , 1f);
            }
        }
        mesh.colors = colors;
    }
    void setMeshRunoffColors(int[] starts, int num, float margin)
    {
        int[] patterns = getRunoffPatterns(starts, num, margin);
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = new Color(0f , 1f * (grid.cells[i].runoffScore / 10), 1f * (grid.cells[i].runoffScore / 10), 1f);
        }
        mesh.colors = colors;
    }

    IEnumerator iterate(int num)
    {
        List<int> startAt = new List<int>();
        for (int j = 0; j < num; j++)
        {
            startAt.Add(UnityEngine.Random.Range(100, 250000));
            setMeshRunoffColors(startAt.ToArray(), 3000, 20f);
            yield return new WaitForSeconds(.01f);
        }
    }

    int[] getRunoffPatterns(int[] startingPoints, int numOfIterations, float margin)
    {
        List<int> patterns = new List<int>();
        List<int> currentPattern = new List<int>();
        foreach (int start in startingPoints)
        {
            patterns.Add(start);
            currentPattern.Clear();
            int previousIndex = 0;
            int ownIndex = start;
            bool keepRolling = true;
            int iteration = 0;
            while (keepRolling == true)
            {
                iteration++;
                
                if (iteration == numOfIterations) { keepRolling = false; }
                float ownHeight = grid.cells[ownIndex].y;
                List<int> possiblePaths = getIndicesOfSurroundingCells(ownIndex, grid, 1);
                int lowestHeightIndex = ownIndex;
                float lowestHeight = ownHeight + margin;
                foreach( int index in possiblePaths)
                { 
                    if (grid.cells[index].y < lowestHeight && index != previousIndex && grid.cells[index].y != 0 && currentPattern.Contains(index) == false)
                    {
                        lowestHeight = grid.cells[index].y;
                        lowestHeightIndex = index;
                    }
                }
                if (lowestHeightIndex == ownIndex) { keepRolling = false; }
                else {
                    grid.cells[lowestHeightIndex].runoffScore += 1;
                    patterns.Add(lowestHeightIndex);
                    currentPattern.Add(lowestHeightIndex);
                    previousIndex = ownIndex;
                    ownIndex = lowestHeightIndex;
                    }
            }
        }
        return patterns.ToArray();
    }

    void computeCurvatureY(Cell cell)
    {
        if (cell.y == 0 || cell.attachedTriangles.Count != 6) 
        {
            cell.curvatureY = 0f;
            return;
        } 
        else 
        {
                Face[] crossingFaces = new Face[2];
                Vector3[] crossingVertices = new Vector3[2];

                int indexNewFace = 0;
                foreach (Triangle triangle in cell.attachedTriangles)
                {
                    foreach (Face face in triangle.faces)
                    {
                        if (((face.startVertex.y < cell.y && face.endVertex.y > cell.y) ||
                            (face.startVertex.y > cell.y && face.endVertex.y < cell.y)) && indexNewFace< 2)
                        {
                            crossingFaces[indexNewFace] = face;
                            indexNewFace++;
                        }
                    }
                }
                int indexNewLoc = 0;
                if(indexNewFace != 2)
                {
                //GameObject dot = Instantiate(dotgreen, cell.position, transform.rotation);
                //Debug.Log("faces" + indexNewFace);
                    cell.curvatureY = 2.5f;
                    return;
                }
                foreach (Face face in crossingFaces)
                {
                    if (face.startVertex.y != 0 && face.endVertex.y != 0)
                    {
                        float xStep = face.startVertex.x - face.endVertex.x;
                        float zStep = face.startVertex.z - face.endVertex.z;

                        float ratio = ((face.startVertex.y - cell.y) / (face.endVertex.y - face.startVertex.y));
                        Vector3 crossPoint = new Vector3(face.startVertex.x + (xStep * ratio), cell.y, face.startVertex.z + (zStep * ratio));
                        crossingVertices[indexNewLoc] = crossPoint;
                        indexNewLoc++;
                    }
                    else
                    {
                        cell.curvatureY = 0;
                        return;
                    }
                }
                Vector3 expectedLocation = new Vector3(((crossingVertices[0].x - crossingVertices[1].x) / 2) + crossingVertices[1].x,
                                                        ((crossingVertices[0].y - crossingVertices[1].y) / 2) + crossingVertices[1].y,
                                                        ((crossingVertices[0].z - crossingVertices[1].z) / 2) + crossingVertices[1].z);
                cell.curvatureY = Mathf.Pow(Mathf.Pow(Vector3.Distance(cell.position, expectedLocation), 2), 0.5f);
            }
    }


    void computeCurvatureX(Cell cell)
    {
        if (cell.y == 0 || cell.attachedTriangles.Count != 6)
        {
            cell.curvatureX = 0f;
            return;
        }
        else
        {
            Cell[] Xcells = new Cell[2];
            Xcells[0] = new Cell(0, vertices[0], normals[0]);
            int index = 0;
      
                foreach (Face face in cell.attachedFaces)
            {
                
                    if (face.endVertex.x == cell.x && face.endVertex.z != cell.z && Xcells[0].index != face.endVertex.index &&  index<2)
                    {
                        Xcells[index] = face.endVertex;
                        index++;
                    }
                    if (face.startVertex.x == cell.x && face.startVertex.z != cell.z && Xcells[0].index != face.startVertex.index && index < 2)
                    {

                        Xcells[index] = face.startVertex;
                        index++;
                    }
                }
           if (index < 2)
                {
                    cell.curvatureX = 0;
                }
                else
                {
                    float avgHeight = (Xcells[0].y + Xcells[1].y) / 2;
                    cell.curvatureX = Mathf.Pow(Mathf.Pow((cell.y - avgHeight), 2), 0.5f);
                }
            
        }
    }

    void computeESRICurvature(Cell cell)
    {
        //https://gis.stackexchange.com/questions/37066/how-to-calculate-terrain-curvature
        //http://help.arcgis.com/en/arcgisdesktop/10.0/help/index.html#//00q90000000t000000
        //D = [(Z4 + Z6) /2 - Z5] / L2
        //E = [(Z2 + Z8) /2 - Z5] / L2
        //The output of the Curvature tool is the second derivative of the surface�for example, the slope of the slope�such that:
        //Curvature = -2(D + E) * 100

        if (cell.y == 0 || cell.attachedTriangles.Count != 6)
        {
            cell.curvature = 0f;
            return;
        }
        else
        {
            Cell[] Xcells = new Cell[2];
            Xcells[0] = new Cell(0, vertices[0], normals[0]);
            Cell[] Zcells = new Cell[2];
            Zcells[0] = new Cell(0, vertices[0], normals[0]);
            int Xindex = 0;
            int Zindex = 0;

            foreach (Face face in cell.attachedFaces)
            {

                if (face.endVertex.x == cell.x && face.endVertex.z != cell.z && Xcells[0].index != face.endVertex.index && Xindex < 2)
                {
                    Xcells[Xindex] = face.endVertex;
                    Xindex++;
                }
                if (face.startVertex.x == cell.x && face.startVertex.z != cell.z && Xcells[0].index != face.startVertex.index && Xindex < 2)
                {

                    Xcells[Xindex] = face.startVertex;
                    Xindex++;
                }
                if (face.endVertex.z == cell.z && face.endVertex.x != cell.x && Zcells[0].index != face.endVertex.index && Zindex < 2)
                {
                    Zcells[Zindex] = face.endVertex;
                    Zindex++;
                }
                if (face.startVertex.z == cell.z && face.startVertex.x != cell.x && Zcells[0].index != face.startVertex.index && Zindex < 2)
                {

                    Zcells[Zindex] = face.startVertex;
                    Zindex++;
                }
            }
            float xStep = cell.attachedFaces[2].startVertex.x - cell.attachedFaces[2].endVertex.x;
            float zStep = cell.attachedFaces[0].startVertex.z - cell.attachedFaces[0].endVertex.z;
            Debug.Log(xStep);
            float D = (((Xcells[0].y + Xcells[1].y) / 2) - cell.y) / (zStep * 2);
            float E = (((Zcells[0].y + Zcells[1].y) / 2) - cell.y) / (xStep * 2);
            cell.curvature = -2 * (D + E);

        }
    }
   

    /*
    void InstantiateRunoff(int[] starts, int num, float margin)
    {
        int[] patterns = getRunoffPatterns(starts, num, margin);
        foreach (int point in patterns)
        {
            GameObject dot = Instantiate(dotgreen, new Vector3(grid.cells[point].x, grid.cells[point].y, grid.cells[point].z),  transform.rotation);
            dot.GetComponent<MeshRenderer>().material.color = new Color((grid.cells[point].runoffScore / 10) * 1f, (grid.cells[point].runoffScore/10) *1f, (grid.cells[point].runoffScore / 10) * 1f, 1f);
            
        }
    }*/
}



