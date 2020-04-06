using System.Collections;
using IPA.Utilities;
using System.Linq;
using UnityEngine;

namespace LevelTweaks
{
    public class LevelTweaker : MonoBehaviour
    {
        public static BeatmapObjectSpawnMovementData _spawnController { get; private set; }
        public float offset;
        public float njs;

        private IEnumerator Load(float o, float n)
        {
            offset = o;
            njs = n;
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().Any());
            _spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First().GetField<BeatmapObjectSpawnMovementData, BeatmapObjectSpawnController>("_beatmapObjectSpawnMovementData");
            ApplyTweaks();
        }

        public void Load(Configuration.TweakData tweak)
        {
            StartCoroutine(Load(tweak.Offset, tweak.NJS));
        }

        public void ApplyTweaks()
        {
            float noteJumpStartBeatOffset = offset;
            float halfJumpDur = 4f;
            float maxHalfJump = _spawnController.GetField<float, BeatmapObjectSpawnMovementData>("_maxHalfJumpDistance");
            float moveSpeed = _spawnController.GetField<float, BeatmapObjectSpawnMovementData>("_moveSpeed");
            float moveDir = _spawnController.GetField<float, BeatmapObjectSpawnMovementData>("_moveDuration");
            float jumpDis;
            float spawnAheadTime;
            float moveDis;
            float bpm = _spawnController.GetField<float, BeatmapObjectSpawnMovementData>("_startBPM");
            float num = 60f / bpm;
            moveDis = moveSpeed * num * moveDir;
            while (njs * num * halfJumpDur > maxHalfJump)
            {
                halfJumpDur /= 2f;
            }
            halfJumpDur += noteJumpStartBeatOffset;
            if (halfJumpDur < 1f) halfJumpDur = 1f;
            jumpDis = njs * num * halfJumpDur * 2f;
            spawnAheadTime = moveDis / moveSpeed + jumpDis * 0.5f / njs;
            _spawnController.SetField("_startHalfJumpDurationInBeats", halfJumpDur);
            _spawnController.SetField("_spawnAheadTime", spawnAheadTime);
            _spawnController.SetField("_jumpDistance", jumpDis);
            _spawnController.SetField("_noteJumpMovementSpeed", njs);
            _spawnController.SetField("_moveDistance", moveDis);
        }
    }
}
