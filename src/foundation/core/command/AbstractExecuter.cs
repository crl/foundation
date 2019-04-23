namespace foundation
{
    public class AbstractExecuter : IExecute
    {
        public string TYPE = "";
        public virtual void execute(params object[] args)
        {
            Executer.execute(this.TYPE, this, args);
        }
    }
}
