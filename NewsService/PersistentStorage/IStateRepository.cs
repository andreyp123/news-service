namespace PersistentStorage
{
    public interface IStateRepository<StateT>
        where StateT : class
    {
        StateT GetState();
        void SetState(StateT state);
    }
}
