using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public class EntityManager
    {
        private static EntityManager instance;
        public static EntityManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityManager();
                }
                return instance;
            }
        }

        public Dictionary<long, Entity> entityDic = new Dictionary<long, Entity>();
        public Dictionary<EntityType, List<Entity>> entityList = new Dictionary<EntityType, List<Entity>>();


        public void Add(Entity entity)
        {
            if (entityDic.ContainsKey(entity.Id))
                return;
            entityDic.Add(entity.Id, entity);
            List<Entity> entitys = null;
            if(entityList.TryGetValue(entity.EType, out entitys))
            {
                entitys.Add(entity);
            }
            else
            {
                entitys = new List<Entity>();
                entitys.Add(entity);
                entityList.Add(entity.EType, entitys);
            }
        }
        public void Remove(Entity entity)
        {
            if(entityDic.Remove(entity.Id))
            {
                List<Entity> entitys = null;
                if (entityList.TryGetValue(entity.EType, out entitys))
                {
                    entitys.Remove(entity);
                }
            }
        }
        public Entity Find(long id)
        {
            Entity entity = null;
            entityDic.TryGetValue(id, out entity);
            return entity;
        }
        public List<Entity> FindByType(EntityType entityType)
        {
            List<Entity> entitys = null;
            entityList.TryGetValue(entityType, out entitys);
            return entitys;
        }
    }
}
