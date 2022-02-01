## SaltStone

ゆっくり動画 作成支援ツール

SaltStoneの紹介です
現在、鋭意作成中です！

<ul>
  {% for post in site.posts %}
    <li>
      <span><b>{{ post.title }}</b><span><span>{{ post.excerpt }}</span>
    </li>
  {% endfor %}
</ul>
