using System;
using System.Linq;
using UnityEngine;


namespace Octrees
{
    public class OctreeAgent : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _accuracy = 1f;
        [SerializeField] private float _turnSpeed = 5f;

        private int _currentWaypoint;
        private OctreeNode _currentNode;
        private Vector3 _destination;

        public OctreeGenerator generator;
        Graph graph;

        private void Start()
        {
            graph = generator.waypoints;
            _currentNode = GetClosestNode(transform.position);
            GetRandomDestination();
        }

        private void Update()
        {
            if (graph == null) return;

            if(graph.GetPathLength() == 0 || _currentWaypoint >= graph.GetPathLength())
            {
                GetRandomDestination();
                return;
            }

            if(Vector3.Distance(graph.GetPathNode(_currentWaypoint).bounds.center, transform.position) < _accuracy)
            {
                _currentWaypoint++;
                //Debug.Log($"Waypoint {_currentWaypoint} reached");
            }

            if(_currentWaypoint <  graph.GetPathLength())
            {
                _currentNode = graph.GetPathNode(_currentWaypoint);
                _destination = _currentNode.bounds.center;

                var dir = _destination - transform.position;
                dir.Normalize();

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _turnSpeed * Time.deltaTime);
                transform.Translate(0, 0, _speed * Time.deltaTime);
            }
        }

        public void GetRandomDestination()
        {
            OctreeNode destNode = null;

            while(!graph.AStar(_currentNode, destNode))
            {
                destNode = graph.nodes.ElementAt(UnityEngine.Random.Range(0, graph.nodes.Count)).Key;
            }

            _currentWaypoint = 0;
            _destination = destNode.bounds.center;
        }

        private OctreeNode GetClosestNode(Vector3 position)
        {
            OctreeNode closestNode = null;

            float closestDistanceSqrd = Mathf.Infinity;

            foreach(var nodePair in graph.nodes)
            {
                var node = nodePair.Key;
                float distanceSqrd = (node.bounds.center - position).sqrMagnitude;
                if (distanceSqrd < closestDistanceSqrd) 
                { 
                    closestNode = node;
                    closestDistanceSqrd = distanceSqrd; 
                }
            }

            return closestNode;
        }

        private void OnDrawGizmos()
        {
            if (graph == null || graph.GetPathLength() == 0) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(graph.GetPathNode(0).bounds.center, 0.7f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(graph.GetPathNode(graph.GetPathLength() - 1).bounds.center, 0.5f);

            Gizmos.color = Color.green;
            for (int i = 0; i < graph.GetPathLength(); i++)
            {
                Gizmos.DrawWireSphere(graph.GetPathNode(i).bounds.center, 0.5f);
                if(i <  graph.GetPathLength() - 1)
                {
                    var start = graph.GetPathNode(i).bounds.center;
                    var end = graph.GetPathNode(i + 1).bounds.center;

                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }
}
