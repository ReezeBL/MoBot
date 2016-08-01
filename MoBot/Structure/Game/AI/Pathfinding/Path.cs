using System.Collections;
using System.Collections.Generic;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    public class Path : IEnumerable<PathPoint>
    {
        readonly IEnumerable<PathPoint> _wayPoints;
        private readonly IEnumerator<PathPoint> _enumerator;
        

        public Path(IEnumerable<PathPoint> list)
        {
            _wayPoints = list;
        }

        public Path(IEnumerator<PathPoint> pathGenerator)
        {
            _enumerator = pathGenerator;
        }       
        public Path()
        {
        }

        public IEnumerator<PathPoint> GetEnumerator()
        {
            return _enumerator ?? _wayPoints.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
