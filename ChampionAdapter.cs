using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;

namespace AppLoLRepo
{
    class ChampionAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PostAdapterClickEventArgs> ItemClick;
        public event EventHandler<PostAdapterClickEventArgs> ItemLongClick;
        List<MainActivity.Champion> items;

        public ChampionAdapter(List<MainActivity.Champion> championList)
        {
            items = championList;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            var id = Resource.Layout.row;
            var itemView = LayoutInflater.From(parent.Context).
                Inflate(id, parent, false);

            var vh = new PostAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as PostAdapterViewHolder;

            // Get image and set
            if (holder != null)
            {
                GetImage(item.Image, holder.ChampionImage);
                // Set other properties
                holder.ChampionName.Text = item.Name;
                holder.ChampionLore.Text = item.Lore;
            }
        }

        // Download champion images
        async void GetImage(string imageUrl, ImageView imageView)
        {
            await ImageService.Instance.LoadUrl(imageUrl)
                .Retry(3, 200)
                .DownSample(400, 400)
                .IntoAsync(imageView);
        }

        public override int ItemCount => items.Count;

        void OnClick(PostAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PostAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class PostAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ImageView ChampionImage { get; set; }
        public TextView ChampionName { get; set; }
        public TextView ChampionLore { get; set; }


        public PostAdapterViewHolder(View itemView, Action<PostAdapterClickEventArgs> clickListener,
                            Action<PostAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            // Get items and set them to properties
            ChampionImage = itemView.FindViewById<ImageView>(Resource.Id.championImage);
            ChampionName = itemView.FindViewById<TextView>(Resource.Id.championTextView);
            ChampionLore = itemView.FindViewById<TextView>(Resource.Id.bodyTextView);

            itemView.Click += (sender, e) => clickListener(new PostAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new PostAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class PostAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}