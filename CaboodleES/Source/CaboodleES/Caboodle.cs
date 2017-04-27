using CaboodleES.Manager;


namespace CaboodleES
{
    /// <summary>
    /// Caboodle holds the entity world
    /// </summary>
    public sealed class Caboodle : ICaboodle
    {
        #region Properties

        public int Id { get { return _id; } }
        public EntityManager Entities { get { return _entityManager; } }
        public SystemManager Systems { get { return _systemsManager; } }
        public EventManager Events { get { return _eventsManager; } }
        internal PoolManager Pool { get { return _poolManager; } }
        
        #endregion

        private readonly EntityManager _entityManager;
        private readonly SystemManager _systemsManager;
        private readonly PoolManager _poolManager;
        private readonly EventManager _eventsManager;
        private readonly int _id;
        private static int _next = 0;


        public Caboodle()
        {
            _id = _next++;
            _poolManager = new PoolManager(this);
            _entityManager = new EntityManager(this); 
            _systemsManager = new SystemManager(this);
            _eventsManager = new EventManager(this);
        }

        public static string GetVersion()
        {
            return global::System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public EntityManager GetEntityManager()
        {
            return _entityManager;
        }

        public SystemManager GetSystemsManager()
        {
            return _systemsManager;
        }

        public void Union(ICaboodle caboodle) { }

        public void Intersection(ICaboodle caboodle) { }

        public void Difference(ICaboodle caboodle) { }

        public void Clear()
        {
            _entityManager.Clear();
        }

        public override string ToString()
        {
            return _entityManager.ToString();
        }
    }
}
