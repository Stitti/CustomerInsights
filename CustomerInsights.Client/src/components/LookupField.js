import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Text, Avatar, Box, Flex, Heading, HoverCard, Link } from "@radix-ui/themes";
export default function LookupField({ title, description, targetUrl, iconUrl, iconFallback }) {
    return (_jsxs(HoverCard.Root, { children: [_jsx(HoverCard.Trigger, { children: _jsx(Link, { href: targetUrl, children: title }) }), _jsx(HoverCard.Content, { maxWidth: "300px", children: _jsxs(Flex, { gap: "4", children: [_jsx(Avatar, { src: iconUrl, fallback: iconFallback }), _jsxs(Box, { children: [_jsx(Heading, { size: "3", as: "h3", children: title }), _jsx(Text, { as: "div", size: "2", children: description })] })] }) })] }));
}
