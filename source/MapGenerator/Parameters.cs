namespace MapGenerator
{
    public class Parameters
    {
        /// <summary>
        /// Final Fantasy XIV installation directory.
        /// </summary>
        public string AssetPath;

        /// <summary>
        /// Territory to generate navigation mesh for.
        /// </summary>
        public int Territory = -1;

        /// <summary>
        /// Generate a single navigation mesh instead of tiled.
        /// </summary>
        public bool SingleNavigationMesh = false;

        /// <summary>
        /// Saves generated terrain collision mesh as Wavefront Obj.
        /// </summary>
        public bool SaveDebugMesh;
    }
}
