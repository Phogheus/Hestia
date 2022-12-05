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
            public const string POLYGON_MUST_HAVE_AT_LEAST_THREE_POINTS_ERROR = "A polygon must contain at least three points.";

            /// <summary>
            /// Error thrown when constructing a triangle with more or less than 3 points,
            /// or when any one point is equal to another defining point
            /// </summary>
            public const string INVALID_TRIANGLE_POINTS_ERROR = "A triangle must be composed of exactly three, non-null, distinct points.";

            /// <summary>
            /// Error thrown when all three points of a triangle are colinear
            /// </summary>
            public const string TRIANGLE_POINTS_CANNOT_BE_COLINEAR_ERROR = "Points of a triangle cannot all be colinear.";
        }
    }
}
