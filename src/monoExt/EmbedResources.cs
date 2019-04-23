using UnityEngine;

namespace foundation
{
    interface IEmbedReorderableListResource
    {
        ReorderableListBase getList();
    }
    public class BaseReorderableListEmbed<T> :MonoBehaviour,IEmbedReorderableListResource where T: ReorderableListBase
    {
        public T list;
        public ReorderableListBase getList()
        {
            return list;
        }
    }
    public class EmbedMaterialsMono: BaseReorderableListEmbed<MaterialReorderableList>
    {
    }
    public class EmbedGameObjectsMono :BaseReorderableListEmbed<GameObjectReorderableList>
    {
    }

    public class EmbedAnimationClipsMono : BaseReorderableListEmbed<AnimationClipReorderableList>
    {
    }

    public class EmbedTexturesMono : BaseReorderableListEmbed<TextureReorderableList>
    {
    }
}