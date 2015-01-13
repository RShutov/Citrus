﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if iOS || ANDROID
using OpenTK.Graphics.ES20;
#elif MAC
using MonoMac.OpenGL;
using PrimitiveType = MonoMac.OpenGL.BeginMode;
#elif WIN
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
#endif
using System.Runtime.InteropServices;

namespace Lime
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	public struct Vertex
	{
		[FieldOffset(0)]
		public Vector2 Pos;

		[FieldOffset(8)]
		public Vector2 UV1;

		[FieldOffset(16)]
		public Color4 Color;

		[FieldOffset(20)]
		public Vector2 UV2;
	}

	public static class VertexBufferPool
	{
		private static Queue<VertexBuffer> items = new Queue<VertexBuffer>(); 

		public static VertexBuffer Acquire()
		{
			if (items.Count * 2 <= VertexBuffer.TotalVertexBuffers) {
				DoubleVertexBuffers();
			}
			return items.Dequeue();
		}

		private static void DoubleVertexBuffers()
		{
			var c = Math.Max(1, VertexBuffer.TotalVertexBuffers);
			for (int i = 0; i < c; i++) {
				items.Enqueue(new VertexBuffer());
			}
		}

		public static void Release(VertexBuffer item)
		{
			item.Clear();
			items.Enqueue(item);
		}
	}

	public unsafe class VertexBuffer : IDisposable, IGLObject
	{
		public const int DefaultCapacity = 1000;
		public readonly int Capacity;
		public Vertex* Vertices;
		public int VertexCount;
		public bool Uploaded;
		/// <summary>
		/// If true, the specially prepared index buffer will be used
		/// </summary>
		public bool SpritesOnly = true;
		public static int TotalVertexBuffers;
		private uint vboHandle;
		private bool disposed;

		public static class Attributes
		{
			public const int Position = 0;
			public const int UV1 = 1;
			public const int UV2 = 2;
			public const int Color = 3;

			public static IEnumerable<ShaderProgram.AttribLocation> GetLocations()
			{
				return new ShaderProgram.AttribLocation[] {
					new ShaderProgram.AttribLocation { Index = VertexBuffer.Attributes.Position, Name = "inPos" },
					new ShaderProgram.AttribLocation { Index = VertexBuffer.Attributes.UV1, Name = "inTexCoords1" },
					new ShaderProgram.AttribLocation { Index = VertexBuffer.Attributes.UV2, Name = "inTexCoords2" },
					new ShaderProgram.AttribLocation { Index = VertexBuffer.Attributes.Color, Name = "inColor" },
				};
			}
		}

		public VertexBuffer(int capacity = DefaultCapacity)
		{
			Capacity = capacity;
			TotalVertexBuffers++;
			Vertices = (Vertex*)Marshal.AllocHGlobal(sizeof(Vertex) * Capacity);
			GLObjectRegistry.Instance.Add(this);
		}

		~VertexBuffer()
		{
			Dispose();
		}

		public void Clear()
		{
			VertexCount = 0;
			Uploaded = false;
			SpritesOnly = true;
		}

		private void AllocateVBOHandle()
		{
			var t = new int[1];
			GL.GenBuffers(1, t);
			vboHandle = (uint)t[0];
			Uploaded = false;
		}

		public void Bind()
		{
			if (vboHandle == 0) {
				AllocateVBOHandle();
			}
			PlatformRenderer.BindVertexBuffer(vboHandle);
			GL.EnableVertexAttribArray(Attributes.Position);
			GL.EnableVertexAttribArray(Attributes.UV1);
			GL.EnableVertexAttribArray(Attributes.Color);
			GL.EnableVertexAttribArray(Attributes.UV2);
			GL.VertexAttribPointer(Attributes.Position, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), (IntPtr)0);
			GL.VertexAttribPointer(Attributes.UV1, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), (IntPtr)8);
			GL.VertexAttribPointer(Attributes.Color, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(Vertex), (IntPtr)16);
			GL.VertexAttribPointer(Attributes.UV2, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), (IntPtr)20);
			if (!Uploaded) {
				Uploaded = true;
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(Vertex) * VertexCount), (IntPtr)Vertices, BufferUsageHint.DynamicDraw);
			}
			PlatformRenderer.CheckErrors();
		}

		public void Dispose()
		{
			if (!disposed) {
				TotalVertexBuffers--;
				Marshal.FreeHGlobal((IntPtr)Vertices);
				Vertices = null;
				Discard();
				disposed = true;
			}
			GC.SuppressFinalize(this);
		}

		public void Discard()
		{
			if (vboHandle != 0) {
				Application.InvokeOnMainThread(() => {
					if (OpenTK.Graphics.GraphicsContext.CurrentContext != null) {
						GL.DeleteBuffers(1, new uint[] { vboHandle });
					}
				});
				vboHandle = 0;
			}
		}
	}
}