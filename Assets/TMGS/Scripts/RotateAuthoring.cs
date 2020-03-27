﻿using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMGS.Rotate
{
    public class RotateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private float degreesPerSecond;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Rotate { radiansPerSecond = math.radians(degreesPerSecond) } );
            dstManager.AddComponentData(entity, new RotationEulerXYZ());
        }
    }
}