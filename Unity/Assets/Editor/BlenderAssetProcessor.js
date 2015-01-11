#pragma strict
 
import System.IO;
import System.Linq;
 
/*
Author: Benjamin Schaaf
*/
class BlenderAssetProcessor extends AssetPostprocessor {
    //After an asset was imported, but before it is saved to disk
    public function OnPostprocessModel(object:GameObject):void {
 
        //only perform corrections with blender files
        var importer : ModelImporter = assetImporter as ModelImporter;
        if (Path.GetExtension(importer.assetPath) == ".blend") {
            RotateObject(object.transform);
        }
 
        //Don't know why we need this...
        //Fixes wrong parent rotation
        object.transform.rotation = Quaternion.identity;
    }
 
    //recursively rotate a object tree individualy
    private function RotateObject(object:Transform):void {
        object.eulerAngles.x += 90;
 
        //if a meshFilter is attached, we rotate the vertex mesh data
        var meshFilter:MeshFilter = object.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (meshFilter) {
            RotateMesh(meshFilter.sharedMesh);
        }
 
        //do this too for all our children
        //Casting is done to get rid of implicit downcast errors
        for (var child:Transform in (object as IEnumerable).Cast.<Transform>()) {
            RotateObject(child);
        }
    }
 
    //"rotate" the mesh data
    private function RotateMesh(mesh:Mesh):void {
        var index:int = 0;
 
        //switch all vertex z values with y values
        var vertices:Vector3[] = mesh.vertices;
        for (index = 0; index < vertices.length; index++) {
            vertices[index] = Vector3(vertices[index].x, vertices[index].z, vertices[index].y);
        }
        mesh.vertices = vertices;
 
        //for each submesh, we invert the order of vertices for all triangles
        //for some reason changing the vertex positions flips all the normals???
        for (var submesh:int = 0; submesh < mesh.subMeshCount; submesh++) {
            var triangles:int[] = mesh.GetTriangles(submesh);
            for (index = 0; index < triangles.length; index += 3) {
                var intermediate:int = triangles[index];
                triangles[index] = triangles[index + 2];
                triangles[index + 2] = intermediate;
            }
            mesh.SetTriangles(triangles, submesh);
        }
 
        //recalculate other relevant mesh data
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}