﻿using UnityEngine;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace GraphProcessor
{
    public class CommentBlockView : Group
	{
		public BaseGraphView	owner;
		public CommentBlock		commentBlock;

        Label                   titleLabel;
        ColorField              colorField;

        public CommentBlockView()
        {
            AddStyleSheetPath("GraphProcessorStyles/CommentBlockView");

            this.AddManipulator(new BorderResizer());
		}

		public void Initialize(BaseGraphView graphView, CommentBlock block)
		{
			commentBlock = block;
			owner = graphView;

            title = block.title;
			SetSize(block.size);
            SetPosition(block.position);

            headerContainer.Q<TextField>().RegisterCallback<ChangeEvent<string>>(TitleChangedCallback);
            titleLabel = headerContainer.Q<Label>();

            colorField = new ColorField{ value = commentBlock.color, name = "headerColorPicker" };
            colorField.OnValueChanged(e =>
            {
                UpdateCommentBlockColor(e.newValue);
            });
            UpdateCommentBlockColor(commentBlock.color);

            headerContainer.Add(colorField);
		}

        public void UpdateCommentBlockColor(Color newColor)
        {
            commentBlock.color = newColor;
            style.backgroundColor = newColor;
            titleLabel.style.textColor = new Color(1 - newColor.r, 1 - newColor.g, 1 - newColor.b, 1);
        }

        void TitleChangedCallback(ChangeEvent< string > e)
        {
            commentBlock.title = e.newValue;
        }
		
		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);

			commentBlock.position = newPos;
		}
	}
}