Site = {}

Site.Utils = function () {
    var AddClassNameToElement = function (element, className) {
        element.className += " " + className;
    };
    var RemoveClassNameFromElement = function (element, className) {
        element.className = element.className.replace(new RegExp("(?:^|\\s)" + className, 'g'), '');
    };

    var SlideDown = function (element) {
        element.style.height = getHeightOfInvisibleElement(element) + 'px';
    }
    var SlideUp = function (element) {
        element.style.height = '0';
    }

    var CheckInView = function (container, element) {
        var cTop = container.scrollTop;
        var cBottom = cTop + container.clientHeight;
        var eTop = element.offsetTop - parseInt(window.getComputedStyle(element).marginTop);
        var eBottom = eTop + element.clientHeight;
        var result = (eTop >= cTop && eBottom <= cBottom);
        return result;
    }
    var ScrollToElement = function (container, element) {
        container.scrollTop = element != null ? element.offsetTop - parseInt(window.getComputedStyle(element).marginTop) : 0;
    }
    var GetScrollParent = function (node) {
        if (node == null) {
            return null;
        }
        if (window.getComputedStyle(node).overflow === 'auto') {
            return node;
        } else {
            return GetScrollParent(node.parentNode);
        }
    }

    var getHeightOfInvisibleElement = function (element) {
        var clonedElement = element.cloneNode(true);
        element.parentNode.appendChild(clonedElement);
        clonedElement.style.position = 'static';
        clonedElement.style.visibility = 'hidden';
        clonedElement.style.display = 'block';
        clonedElement.style.transition = 'none';
        clonedElement.style.height = 'auto';
        var result = clonedElement.offsetHeight;
        element.parentNode.removeChild(clonedElement);
        return result;
    }

    return {
        AddClassNameToElement: AddClassNameToElement,
        RemoveClassNameFromElement: RemoveClassNameFromElement,
        SlideDown: SlideDown,
        SlideUp: SlideUp,
        CheckInView: CheckInView,
        ScrollToElement: ScrollToElement,
        GetScrollParent: GetScrollParent
    }
}();

Site.Search = function () {
    var SELECTED_NAVBAR_ITEM_CLASS_NAME = '.search-result-item-selected';
    var listenerTimeoutID = null;
    var listenerTimeout = 300;
    var lastText = null;
    var scrollableContainer = null;

    var OnSearchBoxGotFocus = function () {
        if (listenerTimeoutID)
            clearTimeout(listenerTimeoutID);
        listenerTimeoutID = setInterval(function () {
            var text = SearchEditor.GetValue();
            if (lastText !== text) {
                lastText = text;
                doSearch(text);
            }
        }, listenerTimeout);
    }

    var OnSearchBoxLostFocus = function () {
        if (listenerTimeoutID)
            clearTimeout(listenerTimeoutID);
        listenerTimeoutID = null;
        if (window.SearchResultsNavBar && SearchResultsNavBar.GetMainElement())
            SearchResultsNavBar.SetSelectedItem(null);
    }

    var OnSearchEditorKeyDown = function (s, e) {
        if (searchResults.GetMainElement().offsetHeight > 0 && window.SearchResultsNavBar && SearchResultsNavBar.GetMainElement()) {
            if (e.htmlEvent.keyCode == 40 || !e.htmlEvent.shiftKey && e.htmlEvent.keyCode == 9) {
                selectItem(true);
                checkScrollPosition();
                preventEvent(e.htmlEvent);
            }
            else if (e.htmlEvent.keyCode == 38 || e.htmlEvent.shiftKey && e.htmlEvent.keyCode == 9) {
                selectItem(false);
                checkScrollPosition();
                preventEvent(e.htmlEvent);
            }
            else if (e.htmlEvent.keyCode == 13) {
                var selectedItem = SearchResultsNavBar.GetSelectedItem();
                if (selectedItem != null)
                    document.location = selectedItem.GetNavigateUrl();
            }
            else if (e.htmlEvent.keyCode == 27) {
                SearchEditor.SetValue(null);
            }
        }
    }

    var checkScrollPosition = function () {
        var container = getScrollableContainer();
        var selectedElement = document.querySelector(SELECTED_NAVBAR_ITEM_CLASS_NAME);
        if (selectedElement == null || !Site.Utils.CheckInView(container, selectedElement))
            Site.Utils.ScrollToElement(container, selectedElement);
    }

    var getScrollableContainer = function () {
        if (!scrollableContainer)
            scrollableContainer = Site.Utils.GetScrollParent(SearchResultsNavBar.GetMainElement());
        return scrollableContainer;
    }

    var preventEvent = function (evt) {
        if (evt.preventDefault)
            evt.preventDefault();
        else
            evt.returnValue = false;
        return false;
    }

    var selectItem = function (next) {
        var selectedItem = SearchResultsNavBar.GetSelectedItem();
        var group = SearchResultsNavBar.GetGroup(0);

        var newIndex = -1;
        if (selectedItem != null) {
            var selectedIndex = selectedItem.index;
            newIndex = next ? selectedIndex + 1 : selectedIndex - 1;
            if (next && newIndex >= group.GetItemCount())
                newIndex = selectedIndex;
            if (!next && newIndex < 0)
                next = 0;
        }
        else if (next) {
            newIndex = 0
        }

        var itemToSelect = group.GetItem(newIndex);
        SearchResultsNavBar.SetSelectedItem(itemToSelect);
    }

    var OnEndCallback = function () {
        setContainerVisiblity(true);
        if (window.SearchResultsNavBar)
            SearchResultsNavBar.SetSelectedItem(null);
    }

    var doSearch = function (text) {
        if (text && text.length > 2) {
            searchResults.PerformCallback(text);
        }
        else
            setContainerVisiblity(false);
    }

    var setContainerVisiblity = function (visible) {
        var element = searchResults.GetMainElement();
        var containerElement = document.querySelector(".search-wrapper");
        if (visible) {
            Site.Utils.SlideDown(element);
        }
        else {
            Site.Utils.SlideUp(element);
        }
    }

    return {
        OnSearchBoxGotFocus: OnSearchBoxGotFocus,
        OnSearchBoxLostFocus: OnSearchBoxLostFocus,
        OnSearchEditorKeyDown: OnSearchEditorKeyDown,
        OnEndCallback: OnEndCallback,
    }
}();

Site.Nav = function () {
    var ToggleNavigationPanel = function () {
        if (window.innerWidth <= NavigationPanel.cpCollapseAtWindowInnerWidth) {
            NavigationPanel.Toggle();
        } else {
            NavigationPanel.SetVisible(!NavigationPanel.GetVisible());
        }
    };

    var NavigationControl = function (treeViewInstance, breadCrumbsButtonInstance, wrapperElementId, breadCrumbsTextElementId) {
        this.ALL_DEMOS_TEXT = 'Menu';
        this.BREAD_CRUMBS_BUTTON_FORVARD_CLASS_NAME = 'arrow-right';
        this.BREAD_CRUMBS_BUTTON_BACKWARD_CLASS_NAME = 'arrow-left';
        this.ROOT_SUBRTEE_MAKRER_CLASS_NAME = 'root-sub-tree';
        this.LIST_ITEM_HOVERED_CLASS_NAME = 'hovered';

        this.treeView = treeViewInstance;
        this.breadCrumbsButton = breadCrumbsButtonInstance;
        this.wrapperElement = document.getElementById(wrapperElementId);
        this.breadText = document.getElementById(breadCrumbsTextElementId);

        this.rootTree = null;
        this.subTree = null;
        this.isRootTreeDisplayed = true;
        this.isMoving = false;

        this.Init = function () {
            this.setSelectedClassToListItems();
            this.addHoverHandlersToListItems();
            this.goToSelectedProductSubTree(true);
            this.wrapperElement.style.visibility = '';
            this.treeView.NodeClick.AddHandler(this.nodeClickHandler.bind(this));
        }

        this.nodeClickHandler = function (s, e) {
            if (!e.node.navigateUrl || e.node.navigateUrl === ASPx.AccessibilityEmptyUrl) {
                if (e.node.parent)
                    e.node.SetExpanded(!e.node.GetExpanded());
                else
                    this.gotToSubTree(e.node);
            }
        }

        this.onNavigationBreadCrumbsButtonClick = function () {
            if (this.isRootTreeDisplayed)
                this.goToSelectedProductSubTree(false);
            else
                this.goToRoot();
        }

        this.goToSelectedProductSubTree = function (quick) {
            var selectedNodeParent = this.getSelectedProductNode();
            if (!selectedNodeParent)
                return;
            this.gotToSubTree(selectedNodeParent, quick);
        }

        this.setSelectedClassToListItems = function () {
            var node = this.treeView.GetSelectedNode();
            while (node != null) {
                var listItemElement = node.GetHtmlElement().parentNode;
                Site.Utils.AddClassNameToElement(listItemElement, 'selected');
                node = node.parent;
            }
        }

        this.getSelectedProductNode = function () {
            var node = this.treeView.GetSelectedNode();
            while (node != null && node.parent != null) {
                node = node.parent;
            }
            return node;
        }

        this.gotToSubTree = function (targetNode, quick) {
            if (this.isMoving)
                return;

            this.isMoving = true;
            this.setBreadCrumbsText(targetNode.text);
            this.breadCrumbsButton.SetText(this.ALL_DEMOS_TEXT);

            this.toggleBreadCrumbsButtonState();

            this.parent = targetNode.GetHtmlElement().parentNode;
            this.subTree = this.parent.getElementsByTagName('UL')[0];
            this.rootTree = this.parent.parentNode;

            var contentDiv = this.treeView.GetControlContentDiv();
            var mainElement = this.treeView.GetMainElement();

            mainElement.style.overflow = 'hidden';

            var savedWidth = contentDiv.style.width;
            contentDiv.style.width = contentDiv.offsetWidth + 'px';
            this.startAnimation(contentDiv, 0, -contentDiv.offsetWidth, function () {
                contentDiv.removeChild(this.rootTree);
                contentDiv.appendChild(this.subTree);
                this.subTree.style.display = '';
                mainElement.style.overflow = 'hidden';
                contentDiv.style.marginLeft = (parseInt(contentDiv.style.marginLeft) * -1) + 'px';
                Site.Utils.RemoveClassNameFromElement(mainElement, this.ROOT_SUBRTEE_MAKRER_CLASS_NAME);
                this.startAnimation(contentDiv, contentDiv.offsetWidth, 0, function () {
                    contentDiv.style.width = savedWidth;
                    this.isRootTreeDisplayed = false;
                    this.isMoving = false;
                }.bind(this), quick);
            }.bind(this), quick);
        }

        this.goToRoot = function () {
            if (this.isMoving)
                return;

            this.isMoving = true;
            var mainElement = this.treeView.GetMainElement();
            var contentDiv = this.treeView.GetControlContentDiv();
            this.breadCrumbsButton.SetText(this.getSelectedProductNode().text);
            this.toggleBreadCrumbsButtonState();

            this.setBreadCrumbsText(this.ALL_DEMOS_TEXT);

            mainElement.style.overflow = 'hidden';

            var savedWidth = contentDiv.style.width;
            contentDiv.style.width = contentDiv.offsetWidth + 'px';
            this.startAnimation(contentDiv, 0, contentDiv.offsetWidth, function () {
                mainElement.style.overflow = '';
                contentDiv.removeChild(this.subTree);
                contentDiv.appendChild(this.rootTree);
                this.parent.appendChild(this.subTree);
                this.subTree.style.display = 'none';
                contentDiv.style.marginLeft = -contentDiv.offsetWidth + 'px';
                Site.Utils.AddClassNameToElement(this.treeView.GetMainElement(), this.ROOT_SUBRTEE_MAKRER_CLASS_NAME);
                this.startAnimation(contentDiv, -contentDiv.offsetWidth, 0, function () {
                    contentDiv.style.width = savedWidth;
                    this.isRootTreeDisplayed = true;
                    this.isMoving = false;
                }.bind(this));
            }.bind(this));
        }

        this.startAnimation = function (element, start, end, onComplete, quick) {
            var duractionMs = 200;
            var savedTransition = element.style.transition;
            if (!quick) {
                element.style.marginLeft = start + 'px';
                element.style.transition = 'margin-left ' + duractionMs / 1000 + 's';
            }
            element.style.marginLeft = end + 'px';
            if (quick)
                onComplete();
            else
                setTimeout(function () {
                    element.style.transition = savedTransition;
                    onComplete();
                }, duractionMs);
        }

        this.setBreadCrumbsText = function (text) {
            this.breadText.innerHTML = text;
        }
        this.toggleBreadCrumbsButtonState = function () {
            var element = this.breadCrumbsButton.GetMainElement().getElementsByClassName("icon")[0];
            var oldCssClass = this.isRootTreeDisplayed ? this.BREAD_CRUMBS_BUTTON_FORVARD_CLASS_NAME : this.BREAD_CRUMBS_BUTTON_BACKWARD_CLASS_NAME;
            var newCssClass = this.isRootTreeDisplayed ? this.BREAD_CRUMBS_BUTTON_BACKWARD_CLASS_NAME : this.BREAD_CRUMBS_BUTTON_FORVARD_CLASS_NAME;
            Site.Utils.RemoveClassNameFromElement(element, oldCssClass);
            Site.Utils.AddClassNameToElement(element, newCssClass);
        }

        this.addHoverHandlersToListItems = function () {
            this.forEachNode(this.treeView, function (node) {
                this.attachHoverHandlers(node);
            }.bind(this));
        }
        this.forEachNode = function (nodeOrTreeView, callback) {
            if (nodeOrTreeView instanceof ASPxClientTreeViewNode)
                callback(nodeOrTreeView);
            var nodesCount = nodeOrTreeView.GetNodeCount();
            for (var i = 0; i < nodesCount; i++) {
                this.forEachNode(nodeOrTreeView.GetNode(i), callback);
            }
        }
        this.attachHoverHandlers = function (node) {
            var listItem = node.GetHtmlElement().parentNode;
            var childElements = listItem.childNodes;
            for (var i = 0; i < childElements.length; i++) {
                var element = childElements[i];
                if (element.tagName === 'UL')
                    continue;

                var onMouseEnter = function () {
                    if (listItem.className.indexOf(this.LIST_ITEM_HOVERED_CLASS_NAME) == -1)
                        Site.Utils.AddClassNameToElement(listItem, this.LIST_ITEM_HOVERED_CLASS_NAME);
                };

                var onMouseLeave = function () {
                    Site.Utils.RemoveClassNameFromElement(listItem, this.LIST_ITEM_HOVERED_CLASS_NAME);
                };

                element.addEventListener('mouseenter', onMouseEnter.bind(this));
                element.addEventListener('mouseleave', onMouseLeave.bind(this));
            }
        }
    }
    return {
        ToggleNavigationPanel: ToggleNavigationPanel,
        NavigationControl: NavigationControl
    }
}();