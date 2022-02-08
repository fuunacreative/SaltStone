#/bin/bash

#vared -p 'comment:' -c msg
echo "comment:"
read -r msg

git add .
git commit -m "$msg"
git push usepush gh-pages
