namespace Assets.Editor
{
    public class PropertyEditor : UnityEditor.Editor
    {
        private PropertyField[] fields;

        public virtual void OnEnable()
        {
            fields = ExposeProperties.GetProperties(target);
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            this.DrawDefaultInspector();

            ExposeProperties.Expose(fields);
        }
    }
}