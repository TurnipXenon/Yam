# This scripts allows us to quickly build on top of the latest branch with our dev branch
git checkout main && git pull origin main && git branch -d turnip/dev && git checkout -b turnip/dev
