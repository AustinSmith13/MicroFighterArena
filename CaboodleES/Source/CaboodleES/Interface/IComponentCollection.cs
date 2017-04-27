namespace CaboodleES.Interface
{
    public interface IComponentCollection
    {
        Component Add(int eid);
        Component Get(int eid);
        Component Remove(int eid);
        int GetId();
        bool Has(int eid);
        void Clear();
        global::System.Type GetCType(); // [DEPRECATED]
    }
}
