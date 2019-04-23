namespace foundation
{
    public class Node<T>
    {
        public Node<T> next;

        public Node<T> prev;

        public T data;

        internal NodeActiveState active = NodeActiveState.Runing;
    }
}