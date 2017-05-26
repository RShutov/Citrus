﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Orange.FbxImporter
{
	public class CameraAttribute : NodeAttribute
	{
		public override FbxNodeType Type { get; } = FbxNodeType.CAMERA;

		public string Name { get; private set; }

		public float FieldOfView { get; private set; }

		public float AspectRatio { get; private set; }

		public float NearClipPlane { get; private set; }

		public float FarClipPlane { get; private set; }

		public CameraAttribute(IntPtr ptr) : base(ptr)
		{
			var cam = FbxNodeGetCameraAttribute(ptr).ToStruct<Camera>();
			Name = cam.name.ToCharArray();
			FieldOfView = cam.fieldOfView;
			AspectRatio = cam.aspectRatio;
			NearClipPlane = cam.clipPlaneNear;
			FarClipPlane = cam.clipPlaneFar;
		}

		#region Pinvokes

		[DllImport(ImportConfig.LibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr FbxNodeGetCameraAttribute(IntPtr node);

		#endregion

		#region MarshalingStructures

		[StructLayout(LayoutKind.Sequential)]
		private class Camera
		{
			public IntPtr name;

			[MarshalAs(UnmanagedType.R4)]
			public float fieldOfView;

			[MarshalAs(UnmanagedType.R4)]
			public float clipPlaneFar;

			[MarshalAs(UnmanagedType.R4)]
			public float clipPlaneNear;

			[MarshalAs(UnmanagedType.R4)]
			public float aspectRatio;
		}

		#endregion
	}
}
