using Yuzu;

namespace Lime
{
	/// <summary>
	/// ����� DistortionMesh
	/// Position ����� ����������� ��� [0,0]-[1,1] (����� ������� � ������ ������ ���� DistortionMesh)
	/// </summary>
	public class DistortionMeshPoint : PointObject
	{
		/// <summary>
		/// ������� (��� ��������� ���� �������� ���������� �� ���� �����)
		/// </summary>
		[YuzuMember]
		public Color4 Color { get; set; }

		/// <summary>
		/// ���������� ����������
		/// </summary>
		[YuzuMember]
		public Vector2 UV { get; set; }

		/// <summary>
		/// �������� ����� ������������ �� ��������� ������� (� ��������)
		/// </summary>
		[YuzuMember]
		public Vector2 Offset { get; set; }

		public DistortionMeshPoint()
		{
			Color = Color4.White;
		}

		/// <summary>
		/// ���������� ������� ����� + ������� DistortionMesh (� ��������)
		/// </summary>
		public Vector2 TransformedPosition {
			get {
				Vector2 result = Offset;
				if (Parent != null && Parent.AsWidget != null) {
					result = Parent.AsWidget.Size * Position + Offset;
				}
				if (SkinningWeights != null && Parent != null && Parent.Parent != null) {
					BoneArray a = Parent.Parent.AsWidget.BoneArray;
					Matrix32 m1 = Parent.AsWidget.CalcLocalToParentTransform();
					Matrix32 m2 = m1.CalcInversed();
					result = m2.TransformVector(a.ApplySkinningToVector(m1.TransformVector(result), SkinningWeights));
				}
				return result;
			}
		}
	}
}
