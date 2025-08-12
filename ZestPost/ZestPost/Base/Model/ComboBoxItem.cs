namespace ZestPost.Base
{
    public class ComboBoxItem<T>
    {
        public T ID { get; set; }
        public string Name { get; set; }

        public ComboBoxItem(T id, string name)
        {
            ID = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
