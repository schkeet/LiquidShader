using UnityEngine;

public class LiquidShaderScript : MonoBehaviour
{
    private Vector3 lastPos;
    private Vector3 velocity;
    private Quaternion lastRot;
    private float angularVelocity;
    [Range(0f, 1f)]
    public float FillAmount = 0.5f;

    [Header("Wobble Settings")]
    public float WobbleSpeedMove = 1f;
    public float Recovery = 1f;
    public Material liquidMaterial;

    private float time;
    private float pulse;
    private float wobbleAmountX;
    private float wobbleAmountZ;
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;

    void Start()
    {
        // Make sure we have a material assigned, otherwise log an error
        if (liquidMaterial == null)
        {
            Debug.LogError("Oops! You forgot to assign the liquid material.");
        }

        // Save the starting position and rotation
        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    float GetAngularVelocity(Quaternion lastRot, Quaternion currentRot)
    {
        float angle = Quaternion.Angle(lastRot, currentRot);
        return angle / Time.deltaTime * 0.01f;
    }

    void Update()
    {
        if (liquidMaterial != null)
        {
            time += Time.deltaTime;

            // Calculate velocities
            velocity = (lastPos - transform.position) / Time.deltaTime;
            angularVelocity = GetAngularVelocity(lastRot, transform.rotation);

            // Add some wobble based on movement and rotation
            wobbleAmountToAddX += (Mathf.Abs(velocity.x) + Mathf.Abs(angularVelocity)) * Time.deltaTime;
            wobbleAmountToAddZ += (Mathf.Abs(velocity.z) + Mathf.Abs(angularVelocity)) * Time.deltaTime;

            // Smoothly reduce the wobble over time
            wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * Recovery);
            wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * Recovery);

            // Wobble add amount clamping
            wobbleAmountToAddX = Mathf.Clamp(wobbleAmountToAddX, -0.8f, 0.8f);
            wobbleAmountToAddZ = Mathf.Clamp(wobbleAmountToAddZ, -0.8f, 0.8f);

            // Sine wave for the wobble effect
            pulse = 2 * Mathf.PI * WobbleSpeedMove;
            wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
            wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

            // Make sure the wobble values don't go too crazy
            wobbleAmountX = Mathf.Clamp(wobbleAmountX, -0.8f, 0.8f);
            wobbleAmountZ = Mathf.Clamp(wobbleAmountZ, -0.8f, 0.8f);

            // Update the shader params
            liquidMaterial.SetFloat("_WaveIntensity", Mathf.Lerp(0.01f, 0.2f, Mathf.Max(Mathf.Abs(wobbleAmountZ), Mathf.Abs(wobbleAmountX)) / 0.8f));
            liquidMaterial.SetFloat("_WobbleX", wobbleAmountZ);
            liquidMaterial.SetFloat("_WobbleY", wobbleAmountX);
            liquidMaterial.SetFloat("_FillAmount", FillAmount);

            // Save position and rotation for the next frame
            lastPos = transform.position;
            lastRot = transform.rotation;
        }
    }
}
