namespace Hestia.Base.Geometry.Utilities
{
    /// <summary>
    /// Defines common constants for geometry content
    /// </summary>
    public static class GeometryConstants
    {
        /// <summary>
        /// Returns the smallest number of points that can constitute a polygon (3: triangle)
        /// </summary>
        public const int MINIMUM_POINT_COUNT_FOR_POLYGON = 3;

        /// <summary>
        /// Defines exception messages that may be thrown
        /// </summary>
        public static class ErrorMessages
        {
            /// <summary>
            /// Error thrown when a polygon is trying to be made with less than three points
            /// </summary>
            public const string POLYGON_MUST_HAVE_AT_LEAST_THREE_POINTS_ERROR = "Any type of polygon must contain at least three distinct points.";

            /// <summary>
            /// Error thrown when all three points of a polygon are colinear
            /// </summary>
            public const string POLYGON_POINTS_CANNOT_BE_COLINEAR_ERROR = "Points of any polygon cannot all be colinear.";
        }
    }
}
