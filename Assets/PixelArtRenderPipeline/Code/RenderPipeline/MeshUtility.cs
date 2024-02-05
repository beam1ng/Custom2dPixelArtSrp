using UnityEngine;

namespace PixelArtRenderPipeline.Code.RenderPipeline
{
    /// <summary>
    /// Provides utility functions for mesh manipulation.
    /// </summary>
    public static class MeshUtility
    {
        /// <summary>
        /// Generates a quad Mesh with a specified width and height.
        /// </summary>
        /// <returns>A Mesh object shaped as a quad.</returns>
        public static Mesh GetQuad()
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0), // Bottom-left
                new Vector3(1, 0, 0), // Bottom-right
                new Vector3(0, 1, 0), // Top-left
                new Vector3(1, 1, 0)  // Top-right
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward, // Facing back
                -Vector3.forward, // Facing back
                -Vector3.forward, // Facing back
                -Vector3.forward  // Facing back
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0), // Bottom-left
                new Vector2(1, 0), // Bottom-right
                new Vector2(0, 1), // Top-left
                new Vector2(1, 1)  // Top-right
            };
            mesh.uv = uv;
            return mesh;
        }
    }
}