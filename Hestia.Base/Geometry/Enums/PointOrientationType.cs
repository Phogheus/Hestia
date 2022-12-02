namespace Hestia.Base.Geometry.Enums
{
    /// <summary>
    /// Defines the types of orientations that can exist between two-dimensional points (and above)
    /// </summary>
    public enum PointOrientationType
    {
        /// <summary>
        /// Colinear (lie on the same line)
        /// </summary>
        Colinear,

        /// <summary>
        /// Clockwise (point is rotated clockwise away)
        /// </summary>
        Clockwise,

        /// <summary>
        /// Counter-Clockwise (point is rotated counter-clockwise away)
        /// </summary>
        CounterClockwise
    }
}
