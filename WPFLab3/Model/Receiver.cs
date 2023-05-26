namespace WPFLab3.Model
{
	public class Receiver
	{
		public Vector3d B { get; set; } = new Vector3d();
		public Vector3d XYZ { get; set; } = new Vector3d();

		public Receiver(Vector3d b, Vector3d XYZ)
		{
			B = b;
			this.XYZ = XYZ;
		}
	}
}
