namespace WorldWind
{
    /// <summary>
    /// Custom vertex for point sprites.
    /// </summary>
    public struct PointSpriteVertex
    {
        public float xval;
        public float yval;
        public float zval;
        public float PointSize;
        public int color;

        public PointSpriteVertex(float fxval, float fyval, float fzval, float psize, int icolor)
        {
            this.xval = fxval;
            this.yval = fyval;
            this.zval = fzval;
            this.PointSize = psize;
            this.color = icolor;
        }
    }
}
