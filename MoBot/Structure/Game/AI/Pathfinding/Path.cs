using System.Collections;
using System.Collections.Generic;

namespace MoBot.Structure.Game.AI.Pathfinding
{
    public class Path : IEnumerable<Location>
    {
        private readonly IEnumerable<Location> _wayPoints;
        private readonly IEnumerator<Location> _enumerator;
        

        public Path(IEnumerable<Location> list)
        {
            _wayPoints = list;
        }

        public Path(IEnumerator<Location> pathGenerator)
        {
            _enumerator = pathGenerator;
        }       
        public Path()
        {
        }

        public IEnumerator<Location> GetEnumerator()
        {
            return _enumerator ?? _wayPoints.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
