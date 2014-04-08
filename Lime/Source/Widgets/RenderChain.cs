﻿using System.Collections.Generic;

namespace Lime
{
	public class RenderChain
	{
		int currentLayer;
		int maxUsedLayer;

		readonly Node[] layers = new Node[Widget.MaxLayer];

		public void Add(Node node, int layer = 0)
		{
			if (layer != 0) {
				int oldLayer = SetCurrentLayer(layer);
				Add(node, 0);
				SetCurrentLayer(oldLayer);
			} else {
				node.NextToRender = layers[currentLayer];
				layers[currentLayer] = node;
			}
		}

		public int SetCurrentLayer(int layer)
		{
			if (layer > maxUsedLayer) {
				maxUsedLayer = layer;
			}
			int oldLayer = currentLayer;
			currentLayer = layer;
			return oldLayer;
		}

		public void RenderAndClear()
		{
			for (int i = 0; i <= maxUsedLayer; i++) {
				Node node = layers[i];
				while (node != null) {
					node.Render();
					Node next = node.NextToRender;
					node.NextToRender = null;
					node = next;
				}
				layers[i] = null;
			}
			maxUsedLayer = 0;
		}
	}
}
