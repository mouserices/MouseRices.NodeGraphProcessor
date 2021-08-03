using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace GraphProcessor
{
	public class EdgeView : Edge
	{
		public bool					isConnected = false;

		public SerializableEdge		serializedEdge { get { return userData as SerializableEdge; } }

		readonly string				edgeStyle = "GraphProcessorStyles/EdgeView";

		protected BaseGraphView		owner => ((input ?? output) as PortView).owner.owner;

		public EdgeView() : base()
		{
			styleSheets.Add(Resources.Load<StyleSheet>(edgeStyle));
			RegisterCallback<MouseDownEvent>(OnMouseDown);
			this.AddManipulator(new FlowPoint());
		}

        public override void OnPortChanged(bool isInput)
		{
			base.OnPortChanged(isInput);
			UpdateEdgeSize();
		}

		public void UpdateEdgeSize()
		{
			if (input == null && output == null)
				return;

			PortData inputPortData = (input as PortView)?.portData;
			PortData outputPortData = (output as PortView)?.portData;

			for (int i = 1; i < 20; i++)
				RemoveFromClassList($"edge_{i}");
			int maxPortSize = Mathf.Max(inputPortData?.sizeInPixel ?? 0, outputPortData?.sizeInPixel ?? 0);
			if (maxPortSize > 0)
				AddToClassList($"edge_{Mathf.Max(1, maxPortSize - 6)}");
		}

		protected override void OnCustomStyleResolved(ICustomStyle styles)
		{
			base.OnCustomStyleResolved(styles);

			UpdateEdgeControl();
		}

		void OnMouseDown(MouseDownEvent e)
		{
			if (e.clickCount == 2)
			{
				// Empirical offset:
				var position = e.mousePosition;
                position += new Vector2(-10f, -28);
                Vector2 mousePos = owner.ChangeCoordinatesTo(owner.contentViewContainer, position);

				owner.AddRelayNode(input as PortView, output as PortView, mousePos);
			}
		}
	}
	
	public class FlowPoint : Manipulator
	{
		VisualElement point { get; set; }

		protected override void RegisterCallbacksOnTarget()
		{
			if (target is Edge edge)
			{
				point = new VisualElement();
				point.AddToClassList("flow-point");
				point.style.left = 0;
				point.style.top = 0;
				target.Add(point);

				target.schedule.Execute(() =>
				{
					UpdateCapPoint(edge, (float)(EditorApplication.timeSinceStartup % 3 / 3));
				}).Until(() => point == null);
			}
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			if (point != null)
			{
				target.Remove(point);
				point = null;
			}
		}

		public void UpdateCapPoint(Edge _edge, float _t)
		{
			Vector3 v = Lerp(_edge.edgeControl.controlPoints, _t);
			point.style.left = v.x;
			point.style.top = v.y;
		}

		Vector2 Lerp(Vector2[] points, float t)
		{
			t = Mathf.Clamp01(t);
			float totalLength = 0;
			for (int i = 0; i < points.Length - 1; i++)
			{
				totalLength += Vector2.Distance(points[i], points[i + 1]);
			}

			float pointLength = Mathf.Lerp(0, totalLength, t);

			float tempLength = 0;
			for (int i = 0; i < points.Length - 1; i++)
			{
				float d = Vector2.Distance(points[i], points[i + 1]);
				if (pointLength <= tempLength + d)
					return Vector2.Lerp(points[i], points[i + 1], (pointLength - tempLength) / d);
				tempLength += d;
			}
			return points[0];
		}
	}
}