/*template <typename T>
class selection {
    T node;
    selection* left;
    selection* right;

public:
    selection()         : node{ }, left{ nullptr }, right{ nullptr } { }
    selection(int i)    : node{ i }, left{ }, right{ } { }
    
    T val() { return node; }
    selection* left()       { return left; }
    selection* right()      { return right; }
    selection* next()       { return left ? left : right; }
    selection* addl(T i)    { left ? left.addl(i) : left = selection<T>(i); }
    selection* addr(T i)    { right ? right.addr(i) : right = selection<T>(i); }
    selection* add(T i)     { left ? right ? nullptr : addr(i) : addl(i); }
};

template<typename T>
selection<T> getNth(size_t n, selection<T> cont) {
    while (n-- > 0 && cont)
        cont = cont.next();
    
    return cont;
}

template<typename T>
T sum(selection<T> cont) {
    return cont.val() + (cont.left() ? sum(cont.left()) : 0) + (cont.right() ? sum(cont.right()) : 0);
}*/