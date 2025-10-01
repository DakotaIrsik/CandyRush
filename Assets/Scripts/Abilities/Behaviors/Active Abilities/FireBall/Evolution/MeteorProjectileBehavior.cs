using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Extensions;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace OctoberStudio.Abilities
{
    public class MeteorProjectileBehavior : MonoBehaviour
    {
        private IAudioManager audioManager;
        private ICameraManager cameraManager;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(IAudioManager audioManager, ICameraManager cameraManager, IEasingManager easingManager)
        {
            this.audioManager = audioManager;
            this.cameraManager = cameraManager;
            this.easingManager = easingManager;
        }
        public static readonly int METEOR_LAUNCH_HASH = "Meteor Launch".GetHashCode();
        public static readonly int METEOR_IMPACT_HASH = "Meteor Impact".GetHashCode();

        [SerializeField] ParticleSystem explosionParticle;
        [SerializeField] GameObject visuals;

        [SerializeField] float speed = 4f;

        public event UnityAction<MeteorProjectileBehavior> onFinished;

        private IEasingCoroutine movementCoroutine;
        private IEasingCoroutine disableCoroutine;

        public float DamageMultiplier { get; set; }
        public float ExplosionRadius { get; set; }

        public void Init(Vector2 impactPosition)
        {
            var spawnPosition = impactPosition + (visuals.transform.rotation * Vector3.up).XY() * cameraManager.HalfHeight * 2.2f;

            visuals.SetActive(true);

            movementCoroutine.StopIfExists();
            disableCoroutine.StopIfExists();

            transform.position = spawnPosition;

            var distance = Vector2.Distance(impactPosition, spawnPosition);
            var duration = distance / speed;
            movementCoroutine = transform.DoPosition(impactPosition, duration).SetOnFinish(Explode);

            audioManager.PlaySound(METEOR_LAUNCH_HASH);
        }

        private void Explode()
        {
            visuals.SetActive(false);

            var enemies = StageController.EnemiesSpawner.GetEnemiesInRadius(transform.position, ExplosionRadius);

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];

                enemy.TakeDamage(PlayerBehavior.Player.Damage * DamageMultiplier);
            }

            explosionParticle.Play();

            disableCoroutine = easingManager.DoAfter(2.5f, () =>
            {
                gameObject.SetActive(false);
                visuals.SetActive(true);
                onFinished?.Invoke(this);
            });

            audioManager.PlaySound(METEOR_IMPACT_HASH);
        }

        public void Clear()
        {
            movementCoroutine.StopIfExists();
            disableCoroutine.StopIfExists();

            gameObject.SetActive(false);

            visuals.SetActive(true);
        }
    }
}