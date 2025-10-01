using UnityEngine;

namespace OctoberStudio.DI
{
    public interface IWorldSpaceTextManager
    {
        void ShowDamageText(Vector2 position, float damage);
        void ShowHealText(Vector2 position, float heal);
    }
}