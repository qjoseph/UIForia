using UIForia.Util;
using System;
using JetBrains.Annotations;

namespace UIForia.Routing {

    [TemplateTagName("ChildRouter")]
    public class ChildRouterElement : RouteElement, IRouterElement {

        [PublicAPI]
        public event Action onRouteChanged;

        protected LightList<RouteElement> m_ChildRoutes;
        protected RouteElement parentRoute;
        protected RouteElement activeChild;

        public ChildRouterElement() {
            m_ChildRoutes = new LightList<RouteElement>(4);
        }

        public RouteElement ActiveChild => activeChild;

        // todo -- remove this allocation
        public override string FullPath => parentRoute.FullPath + path;
        
        public override void OnCreate() {
            UIElement ptr = parent;
            while (ptr != null) {
                if (ptr is RouteElement routeParent) {
                    parentRoute = routeParent;
                    break;
                }

                ptr = ptr.parent;
            }

            parentRoute.onEnter += HandleParentEnter;
            parentRoute.onExit += HandleParentExit;
            parentRoute.onUpdate += HandleParentUpdate;
        }

        protected void HandleParentEnter() {
            if (string.IsNullOrEmpty(path)) {
                Enter(parentRoute.CurrentMatch);
            }
            else {
                match = RouteMatch.Match(FullPath, 0, new RouteMatch(parentRoute.CurrentMatch.url));
                if (match.IsMatch) {
                    Enter(match);
                }
            }
        }

        protected void HandleParentExit() {
            if (activeChild != null) {
                activeChild.Exit();
                activeChild = null;
            }
        }

        protected void HandleParentUpdate() {

            bool wasMatched = IsRouteMatched;
            
            match = RouteMatch.Match(FullPath, 0, new RouteMatch(parentRoute.CurrentMatch.url));
            
            if (!wasMatched && match.IsMatch) {
                Enter(match);
            }
            else if (wasMatched && !match.IsMatch) {
                Exit();
            }
            else if (wasMatched && match.IsMatch) {

                RouteMatch route = string.IsNullOrEmpty(path) ? parentRoute.CurrentMatch : CurrentMatch;
                Update(route); // todo -- not sure about this
                RouteElement[] routes = m_ChildRoutes.List;
                for (int i = 0; i < m_ChildRoutes.Length; i++) {
                    RouteMatch childMatch;
                    if (routes[i].TryMatch(route, out childMatch)) {
                        if (activeChild == routes[i]) {
                            activeChild.Update(childMatch);
                            return;
                        }

                        activeChild?.Exit();
                        activeChild = routes[i];
                        activeChild.Enter(childMatch);
                        onRouteChanged?.Invoke();
                        return;
                    }
                }

                if (activeChild != null) {
                    activeChild.Exit();
                    activeChild = null;
                }

                onRouteChanged?.Invoke();
            }
        }

        public override bool TryMatch(RouteMatch match, out RouteMatch result) {
            result = RouteMatch.Match(FullPath, 0, match);
            return result.IsMatch;
        }

        public override void Enter(RouteMatch route) {
            base.Enter(route);
            RouteElement[] routes = m_ChildRoutes.List;
            for (int i = 0; i < m_ChildRoutes.Length; i++) {
                RouteMatch childMatch;
                if (routes[i].TryMatch(route, out childMatch)) {
                    activeChild = routes[i];
                    routes[i].Enter(childMatch);
                    break;
                }
            }
        }

        public void AddChildRoute(RouteElement routeElement) {
            if (m_ChildRoutes.Contains(routeElement)) {
                return;
            }

            m_ChildRoutes.Add(routeElement);
            routeElement.SetEnabled(false);
        }

        public void RemoveChildRoute(RouteElement routeElement) {
            m_ChildRoutes.Remove(routeElement);
            if (activeChild == routeElement) {
                activeChild = null;
            }
        }

    }

}