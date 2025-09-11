import requests
import os

REPO = "vik37/Orchware-Public"
LABELS = {
    "todo": "blue",
    "in-progress": "yellow",
    "qa-test": "pink",
    "done": "lightgrey",
    "completed": "purple"
}

def count_issues(label):
    url = f"https://api.github.com/search/issues?q=repo:{REPO}+label:status:{label}+state:open"
    headers = {"Authorization": f"Bearer {os.getenv('GH_ISS')}"}
    response = requests.get(url, headers=headers)
    if response.status_code != 200:
        print(f"Error fetching {label}: {response.status_code}")
        return 0
    return response.json().get("total_count", 0)

def generate_badges():
    lines = ["## 📊 Issue Count by Status <!-- STATUS START -->\n"]
    for label, color in LABELS.items():
        count = count_issues(label)
        badge = f"![{label.capitalize()}](https://img.shields.io/badge/{label.replace('-', '--')}-{count}-{color}?style=flat-square)"
        lines.append(badge)
    lines.append("\n<!-- STATUS END -->")
    return "\n".join(lines)

def update_readme():
    with open("README.md", "r", encoding="utf-8") as f:
        content = f.read()

    start = "<!-- STATUS START -->"
    end = "<!-- STATUS END -->"

    if start not in content or end not in content:
        print("Markers not found in README.md")
        return

    before = content.split(start)[0]
    after = content.split(end)[1]
    middle = generate_badges()

    new_content = f"{before}{middle}{after}"
    with open("README.md", "w", encoding="utf-8") as f:
        f.write(new_content)

update_readme()
